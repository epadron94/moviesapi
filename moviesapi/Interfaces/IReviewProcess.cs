namespace moviesapi.Interfaces;
using moviesapi.Models.Dto;

public interface IReviewProcess
{
    Task<ReviewDto> GetReviewAsync(string id);
    Task<MovieReviewsDto> GetMovieReviewsAsync(string movieId);
    Task<UserReviewsDto> GetUserReviewsAsync(string userId);
    Task<bool> PostReviewAsync(ReviewDto review);
    Task<ReviewDto> PatchReviewAsync(string id, ReviewDto review);
    Task<bool> DeleteReviewAsync(string id);


}