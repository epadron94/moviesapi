namespace moviesapi.Interfaces;
using moviesapi.Models.Dto;
using Microsoft.Azure.Cosmos;
using moviesapi.Models.Responses;

public interface IReviewProcess
{
    Task<ReviewResponse> GetReviewAsync(Guid reviewId,Guid movieId);
    Task<ReviewResponse> PostReviewAsync(ReviewDto review);
    Task<ReviewResponse> PatchReviewAsync(ReviewDto review);
    Task<ReviewResponse> DeleteReviewAsync(Guid id, Guid movieId);
    Task<(List<ReviewDto>, string ContinuationToken, double requestCharge)> GetMovieReviewsAsync(int pageSize, string continuationToken, Guid movieId);
    Task<bool> ReviewExists(Guid reviewId,Guid movieId);

}