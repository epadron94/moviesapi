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

        //1.validate movieIdExists
        bool movieExists = await unitOfWork.MovieProcess.ItemExistsAsync(review.MovieId);
        if(!movieExists) throw new ArgumentException("MovieId not found");

        double totalRequestCharge = 0;
        //2.Post Review (review container)
        var postReviewTsk =  unitOfWork.ReviewProcess.PostReviewAsync(review);
        
        //totalRequestCharge += response.RequestCharge;
        //3. Post User review(user container)
        var postUserReviewTsk = unitOfWork.UserProcess.postUserReview(review);

        var responses = await Task.WhenAll(postReviewTsk, postUserReviewTsk);
        //totalRequestCharge += userResponse.RequestCharge;
        //4. Post Movie review(movie container)
        //var movieResponse = await unitOfWork.MovieProcess.PostMovieReview(review);
        //totalRequestCharge +=movieResponse.RequestCharge;
        return (responses[0].StatusCode, totalRequestCharge);        
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

}