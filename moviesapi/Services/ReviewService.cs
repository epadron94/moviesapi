namespace moviesapi.Services;
using moviesapi.Process;
using moviesapi.Interfaces;
using moviesapi.Models.Dto;
using System.Net;
using moviesapi.Models;

public class ReviewService
{
    private readonly IUnitOfWork unitOfWork;

    public ReviewService(IUnitOfWork _unitOfWork)
    {
        unitOfWork = _unitOfWork;
    }

    public async Task<HttpStatusCode> postReview(ReviewDto review)
    {

        //1.validate movieIdExists
        bool movieExists = await unitOfWork.MovieProcess.ItemExistsAsync(review.MovieId);
        if(!movieExists) throw new ArgumentException("MovieId not found");

        //2.Post Review (review container)
        var response = await unitOfWork.ReviewProcess.PostReviewAsync(review);

        //3. Post User review(user container)
        var userResponse = await unitOfWork.UserProcess.postUserReview(review);

        //4. Post Movie review(movie container)
        var movieResponse = await unitOfWork.MovieProcess.PostMovieReview(review);
        
        return response.StatusCode;
        
        
    }

}