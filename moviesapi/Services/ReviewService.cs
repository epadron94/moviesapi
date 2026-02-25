namespace moviesapi.Services;
using moviesapi.Process;
using moviesapi.Interfaces;
using moviesapi.Models.Dto;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

public class ReviewService
{
    private readonly IUnitOfWork unitOfWork;

    public ReviewService(IUnitOfWork _unitOfWork)
    {
        unitOfWork = _unitOfWork;
    }

    public async Task<(HttpStatusCode statusCode, double requestCharge)> postReview(ReviewDto review)
    {
        /*This is a first version for storing reviews, as is needed  query reviews by movie and by user,
        in order to avoid cross-partition querys and save RUs, I've decided to duplicate the document in both containers,
        in addition... to ensure data integrity  when saving data I've built this 'transaction', 
        in the future I'll explore how to improve this component using cosmosdb change feed through an AZF
        is the "cosmos way" to work with multiple operations in cosmosdb, and I need to learn about it*/

        double totalRequestCharge = 0;
        try
        {
            //1.validate movieIdExists
            bool movieExists = await unitOfWork.MovieProcess.ItemExistsAsync(review.MovieId);
            if(!movieExists) 
                throw new ArgumentException("MovieId not found");
            
            //2.Post Review (review container)      
            var postReviewTsk = await  unitOfWork.ReviewProcess.PostReviewAsync(review);
            //As any change was made in the container, only throw the exception and exit
            if(postReviewTsk.GetException is not null)
                throw postReviewTsk.GetException;
            //if success, retrieve operation charge
            totalRequestCharge += postReviewTsk.GetReview.RequestCharge;
            //3. Post Review User container
            var postUserReviewTsk = await unitOfWork.UserProcess.postUserReview(review);
            if(postUserReviewTsk.GetException is not null)
            {
                //ROLLBACK TRANSACTION
                // at this point, as post in User container failed and the update was not made, rollback in review container
                await DeleteReview(review.ReviewId, review.MovieId);
                throw postUserReviewTsk.GetException;
            }            
            totalRequestCharge += postUserReviewTsk.GetReview.RequestCharge;    
        }
        catch(Exception ex ) 
        {
            throw ex;
        }
        return (HttpStatusCode.OK, totalRequestCharge);        
    }

    public async Task<(HttpStatusCode code , double requestCharge)> PatchReview(ReviewDto review)
    {
        double totalRequestCharge = 0;
        try
        {
            //1.validate movieIdExists
            bool movieExists = await unitOfWork.MovieProcess.ItemExistsAsync(review.MovieId);
            if(!movieExists) throw new ArgumentException("MovieId not found");
            //2.Patch Review Container
            var reviewResponse = await unitOfWork.ReviewProcess.PatchReviewAsync(review);
            if(reviewResponse.GetException is not null)
                throw reviewResponse.GetException;
            totalRequestCharge +=reviewResponse.GetReview.RequestCharge;
            //3.Patch User Container
            var userResponse = await unitOfWork.UserProcess.PatchUserReviewAsync(review);
            if(userResponse.GetException is not null)
            {
                //ROLLBACK TRANSACTION
                //at this point the patch operation was nos succesfull in User container, take the document in this container and patch in Review container
                await RollbackReview(review.ReviewId, review.MovieId);
                throw userResponse.GetException;
            }
                
            totalRequestCharge += userResponse.GetReview.RequestCharge;
        }
        catch(Exception ex)
        {
           throw ex; 
        }
        
        return  (HttpStatusCode.OK, totalRequestCharge);
    }

    private async Task DeleteReview(Guid reviewId, Guid movieId)
    {
        try
        {
            var review = await unitOfWork.ReviewProcess.DeleteReviewAsync(reviewId, movieId);
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    private async Task RollbackReview(Guid reviewId, Guid userId)
    {
        var review = await unitOfWork.UserProcess.GetReview(reviewId, userId);
        _ = await unitOfWork.ReviewProcess.PatchReviewAsync(review.Resource);
    }
    public async Task<HttpStatusCode> DeleteReview(Guid reviewId, Guid movieId, Guid userId)
    {
        try
        {
            var reviewExists = await unitOfWork.ReviewProcess.ReviewExists(reviewId, movieId);
            if(!reviewExists)
                throw new ArgumentException("Review not found");

            var deleteMovieReview = await unitOfWork.ReviewProcess.DeleteReviewAsync(reviewId,movieId);
            var deleteUserReview = await unitOfWork.UserProcess.DeleteUserReview(reviewId, userId);
            return HttpStatusCode.OK;
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }



}