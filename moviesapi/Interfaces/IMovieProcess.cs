namespace moviesapi.Interfaces;
using moviesapi.Models.Dto;
using Microsoft.Azure.Cosmos;

public interface IMovieProcess
{
    Task<(List<MovieDto>, string ContinuationToken, double cost)> GetAllItemsAsync(int ? pageSize, string continuationToken);
    Task<MovieDto> GetMovieById(string Id);
    Task<bool> ItemExistsAsync(Guid id);
    Task<ItemResponse<ReviewDto>> PostMovieReview(ReviewDto review);
    Task<(List<ReviewDto>, string ContinuationToken)> GetMovieReviewsAsync(int pageSize, string continuationToken, Guid movieId);
}
