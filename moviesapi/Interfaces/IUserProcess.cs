namespace moviesapi.Interfaces;
using Microsoft.Azure.Cosmos;
using moviesapi.Models.Dto;
public interface IUserProcess
{
    Task<ItemResponse<ReviewDto>> postUserReview(ReviewDto userReview);
    Task<(List<ReviewDto>,string, double)> GetUserReviews(int pageSize, Guid userId, string continuationToken=null);
    Task<ItemResponse<ReviewDto>> PatchUserReviewAsync(ReviewDto review);
}