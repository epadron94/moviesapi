namespace moviesapi.Process;
using moviesapi.Services;
using  _cosmos=Microsoft.Azure.Cosmos;
using moviesapi.Models;
using moviesapi.Interfaces;
using moviesapi.Models.Dto;
using moviesapi.Utilities;
using moviesapi.Models.Responses;

public class ReviewProcess : IReviewProcess
{
    private readonly CosmosDbService cosmosService;
    private readonly _cosmos.Container container;
    private readonly Utilities utilities;

    public ReviewProcess(CosmosDbService service, Utilities _utilities)
    {
        cosmosService = service;
        container = cosmosService.GetContainerInstance<Review>();
        utilities =_utilities;

    }
    public Task<ReviewDto> GetReviewAsync(string id)
    {
        throw new NotImplementedException();
    }
    public async Task<ReviewResponse> PostReviewAsync(ReviewDto review)
    {
        try
        {
            var response = await container.CreateItemAsync(review, new _cosmos.PartitionKey(review.MovieId.ToString()));
            return new ReviewResponse(response);
        }
        catch(_cosmos.CosmosException ex)
        {
            return new ReviewResponse(ex);
        }
        //throw new _cosmos.CosmosException("test- Request was throttled", HttpStatusCode.TooManyRequests,0,string.Empty, 0);
        
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
    public async Task<bool> DeleteReviewAsync(Guid reviewId, Guid movieId)
    {
        var response = await container.DeleteItemAsync<ReviewDto>(reviewId.ToString(),new _cosmos.PartitionKey(movieId.ToString()));
        return true;
    }
    public async Task<(List<ReviewDto>, string ContinuationToken, double requestCharge)> GetMovieReviewsAsync(int pageSize, string continuationToken, Guid movieId)
    {
        var result = new List<ReviewDto>();
        var token = utilities.Decode(continuationToken);
        var query = new _cosmos.QueryDefinition("SELECT c.id, c.userId, c.reviewId, c.rating, c.review, c.reviewDate, c.movieId from c WHERE c.entityType=@entityType AND c.movieId=@movieId")
                    .WithParameter("@entityType","review")
                    .WithParameter("@movieId", Convert.ToString(movieId));

        var opts = new _cosmos.QueryRequestOptions
        {
            MaxItemCount = pageSize,
            PartitionKey = new _cosmos.PartitionKey(Convert.ToString(movieId))
        };
        var iterator = container.GetItemQueryIterator<ReviewDto>(query,token,opts);
        var response = await iterator.ReadNextAsync();
        result.AddRange(response.Resource);

        string continuationTokenEncoded = utilities.Encode(response.ContinuationToken);
        return (result, continuationTokenEncoded,response.RequestCharge);

    }

}