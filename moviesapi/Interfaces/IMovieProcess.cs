namespace moviesapi.Interfaces;
using moviesapi.Models.Dto;
using Microsoft.Azure.Cosmos;

public interface IMovieProcess
{
    
    Task<(List<MovieDto>, string ContinuationToken, double cost)> GetAllItemsAsync(int ? pageSize, string continuationToken);
    Task<(MovieDto, double)> GetMovieById(string Id);
    Task<(bool, double)> MovieExistsAsync(Guid movieId);
    Task<ItemResponse<ReviewDto>> PostMovieReview(ReviewDto review);
    Task<ItemResponse<ReviewDto>> PatchMovieReviewAsync(ReviewDto review);
}
