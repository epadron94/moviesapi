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
                await deleteReview(review.ReviewId, review.MovieId);
                throw postUserReviewTsk.GetException;
            }
                
            
            totalRequestCharge += postUserReviewTsk.GetReview.RequestCharge;    
        }
        catch(Exception ex ) 
        {
            throw ex;
        }
        
        //4. Post Movie review(movie container)
        //var movieResponse = await unitOfWork.MovieProcess.PostMovieReview(review);
        //totalRequestCharge +=movieResponse.RequestCharge;
        return (HttpStatusCode.OK, totalRequestCharge);        
    }

    public async Task<(HttpStatusCode code , double requestCharge)> PatchReview(ReviewDto review)
    {
        //1.validate movieIdExists
        bool movieExists = await unitOfWork.MovieProcess.ItemExistsAsync(review.MovieId);
        if(!movieExists) throw new ArgumentException("MovieId not found");
        double totalRequestCharge = 0;
        //2.PAtch Review Container
        var reviewResponse = await unitOfWork.ReviewProcess.PatchReviewAsync(review);
        totalRequestCharge +=reviewResponse.RequestCharge;
        //3.Patch Movie Container
        var movieResponse = await unitOfWork.MovieProcess.PatchMovieReviewAsync(review);
        totalRequestCharge +=movieResponse.RequestCharge;
        //4.Patch User Container
        //var userResponse = await unitOfWork.UserProcess.PatchUserReviewAsync(review);
        //totalRequestCharge += userResponse.RequestCharge;
        return  (reviewResponse.StatusCode, totalRequestCharge);
    }

    private async Task deleteReview(Guid reviewId, Guid movieId)
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
}