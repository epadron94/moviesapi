namespace moviesapi.Interfaces;
using Microsoft.Azure.Cosmos;
using moviesapi.Models.Dto;
public interface IUserProcess
{
    Task<ItemResponse<ReviewDto>> postUserReview(ReviewDto userReview);
}