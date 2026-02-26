namespace moviesapi.Interfaces;
using Microsoft.Azure.Cosmos;
using moviesapi.Models.Dto;
using moviesapi.Models.Responses;
public interface IUserProcess
{
    Task<ReviewResponse> postUserReview(ReviewDto userReview);
    Task<(List<ReviewDto>,string, double)> GetUserReviews(int pageSize, Guid userId, string continuationToken=null);
    Task<ReviewResponse> PatchUserReviewAsync(ReviewDto review);
    Task<ReviewResponse> DeleteUserReview(Guid reviewId, Guid userId);
    Task<ReviewResponse> GetReviewAsync(Guid reviewId, Guid userId);
}