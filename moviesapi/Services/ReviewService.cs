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
            var (movieExists, requestCharge) = await unitOfWork.MovieProcess.MovieExistsAsync(review.MovieId);
            if(!movieExists) 
                throw new ArgumentException("MovieId not found");
            totalRequestCharge += requestCharge;
            //2.Post Review (review container)      
            var postReviewTsk = await  unitOfWork.ReviewProcess.PostReviewAsync(review);
            //As any change was made in the container, only throw the exception and exit
            if(postReviewTsk.IsError)
                throw postReviewTsk.GetException;
            //if success, retrieve operation charge
            totalRequestCharge += postReviewTsk.GetRequestCharge;
            //3. Post Review User container
            var postUserReviewTsk = await unitOfWork.UserProcess.postUserReview(review);
            if(postUserReviewTsk.IsError)
            {
                //ROLLBACK TRANSACTION
                // at this point, as post in User container failed and the update was not made, rollback in review container
                _ = await RollbackPostReview(review.ReviewId, review.MovieId);
                //log the requestCharge
                throw postUserReviewTsk.GetException;
            }
            else
                totalRequestCharge += postUserReviewTsk.GetRequestCharge;    
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
            var (movieExists, requestCharge) = await unitOfWork.MovieProcess.MovieExistsAsync(review.MovieId);
            if(!movieExists) 
                throw new ArgumentException("MovieId not found");
            totalRequestCharge += requestCharge;
            //2.Patch Review Container
            var reviewResponse = await unitOfWork.ReviewProcess.PatchReviewAsync(review);
            if(reviewResponse.IsError)
                throw reviewResponse.GetException;
            totalRequestCharge +=reviewResponse.GetRequestCharge;
            //3.Patch User Container
            var userResponse = await unitOfWork.UserProcess.PatchUserReviewAsync(review);
            if(userResponse.IsError)
            {
                //ROLLBACK TRANSACTION
                //at this point the patch operation was nos succesfull in User container, take the document in this container and patch in Review container
                double rollbackRequestCharge = await RollbackPatchReview(review.ReviewId, review.UserId);
                totalRequestCharge += rollbackRequestCharge;
                throw userResponse.GetException;
            }
            else
                totalRequestCharge += userResponse.GetRequestCharge;
        }
        catch(Exception ex)
        {
            throw ex;
        }
        
        return  (HttpStatusCode.OK, totalRequestCharge);
    }

    private async Task<double> RollbackPostReview(Guid reviewId, Guid movieId)
    {
        double requestCharge = 0;
        try
        {
            var review = await unitOfWork.ReviewProcess.DeleteReviewAsync(reviewId, movieId);
            if(review.IsError)
                throw review.GetException;
            requestCharge += review.GetRequestCharge;
            return requestCharge;
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    private async Task<double> RollbackPatchReview(Guid reviewId, Guid userId)
    {
        double requestCharge = 0;
        try
        {
            var review = await unitOfWork.UserProcess.GetReviewAsync(reviewId, userId);
            if(review.IsError)
                throw review.GetException; //something really bad is happening at this point, call god, do something!

            requestCharge += review.GetRequestCharge;
            var patchReview = await unitOfWork.ReviewProcess.PatchReviewAsync(review.GetReviewDto);
            if(patchReview.IsError)
                throw patchReview.GetException;//the rollback failed, data integrity is compromised ad this point, hope change feed solve this
            requestCharge += patchReview.GetRequestCharge;
            return requestCharge;
        }
        catch(Exception ex)
        {
            throw;
        }
        
    }
    public async Task<HttpStatusCode> DeleteReview(Guid reviewId, Guid movieId, Guid userId)
    {
        try
        {
            var reviewExists = await unitOfWork.ReviewProcess.ReviewExists(reviewId, movieId);
            if(!reviewExists)
                throw new ArgumentException("Review not found");

            var deleteMovieReview = await unitOfWork.ReviewProcess.DeleteReviewAsync(reviewId,movieId);
            if(deleteMovieReview.IsError)
                throw  deleteMovieReview.GetException;
            var deleteUserReview = await unitOfWork.UserProcess.DeleteUserReview(reviewId, userId);
            if(deleteUserReview.IsError)
            {
                //ROLLBACK DELETE, 
                RollbackDeleteReview(reviewId, userId);
                throw deleteUserReview.GetException;
            }
            return HttpStatusCode.OK;
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    private async Task RollbackDeleteReview(Guid reviewId, Guid userId)
    {
        var review = await unitOfWork.UserProcess.GetReviewAsync(reviewId, userId);
        _= await unitOfWork.ReviewProcess.PostReviewAsync(review.GetReviewDto);
    }



}