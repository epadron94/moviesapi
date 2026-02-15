namespace moviesapi.Process;
using moviesapi.Services;
using  _cosmos=Microsoft.Azure.Cosmos;
using moviesapi.Models;
using moviesapi.Interfaces;
using moviesapi.Models.Dto;

public class ReviewProcess : IReviewProcess
{
    private readonly CosmosDbService cosmosService;
    private readonly _cosmos.Container container;

    public ReviewProcess(CosmosDbService service)
    {
        cosmosService = service;
        container = cosmosService.GetContainerInstance<Review>();
    }

    public Task<ReviewDto> GetReviewAsync(string id)
    {
        throw new NotImplementedException();
    }
    public Task<MovieReviewDto> GetMovieReviewsAsync(string movieId)
    {
        throw new NotImplementedException();
    }
    public Task<UserReviewsDto> GetUserReviewsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<_cosmos.ItemResponse<ReviewDto>> PostReviewAsync(ReviewDto review)
    {
        var response = await container.CreateItemAsync(review, new _cosmos.PartitionKey(Convert.ToString(review.Id)));
        return response;
    }
    public Task<ReviewDto> PatchReviewAsync(string id, ReviewDto review)
    {
        throw new NotImplementedException();
    }
    public Task<bool> DeleteReviewAsync(string id)
    {
        throw new NotImplementedException();
    }



}