namespace moviesapi.Interfaces;
using moviesapi.Models.Dto;
using Microsoft.Azure.Cosmos;

public interface IReviewProcess
{
    Task<ReviewDto> GetReviewAsync(string id);
    Task<MovieReviewDto> GetMovieReviewsAsync(string movieId);
    Task<UserReviewsDto> GetUserReviewsAsync(string userId);
    Task<ItemResponse<ReviewDto>> PostReviewAsync(ReviewDto review);
    Task<ReviewDto> PatchReviewAsync(string id, ReviewDto review);
    Task<bool> DeleteReviewAsync(string id);


}