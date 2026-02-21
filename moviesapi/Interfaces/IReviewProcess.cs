namespace moviesapi.Interfaces;
using moviesapi.Models.Dto;
using Microsoft.Azure.Cosmos;

public interface IReviewProcess
{
    Task<ReviewDto> GetReviewAsync(string id);
    Task<ItemResponse<ReviewDto>> PostReviewAsync(ReviewDto review);
    Task<ItemResponse<ReviewDto>> PatchReviewAsync(ReviewDto review);
    Task<bool> DeleteReviewAsync(string id);


}