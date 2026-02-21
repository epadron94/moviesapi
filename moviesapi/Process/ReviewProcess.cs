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

    public async Task<_cosmos.ItemResponse<ReviewDto>> PostReviewAsync(ReviewDto review)
    {
        var response = await container.CreateItemAsync(review, new _cosmos.PartitionKey(review.MovieId.ToString()));
        return response;
    }
    public async Task<_cosmos.ItemResponse<ReviewDto>> PatchReviewAsync(ReviewDto review)
    {
        //var response = await container.ReplaceItemAsync(review,Convert.ToString(review.ReviewId), new _cosmos.PartitionKey(Convert.ToString(review.ReviewId)));
        var patchOp = new List<_cosmos.PatchOperation>
        {
            _cosmos.PatchOperation.Replace("/rating", review.Rating),
            _cosmos.PatchOperation.Replace("/review",review.Review),
            _cosmos.PatchOperation.Replace("/reviewDate", DateTime.UtcNow)
        };
        var response = await container.PatchItemAsync<ReviewDto>(review.ReviewId.ToString(),
                                                                new _cosmos.PartitionKey(review.MovieId.ToString()),
                                                                patchOp);
        return response;
    }
    public Task<bool> DeleteReviewAsync(string id)
    {
        throw new NotImplementedException();
    }



}