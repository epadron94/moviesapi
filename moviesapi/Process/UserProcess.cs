namespace moviesapi.Process;
using Microsoft.Azure.Cosmos;
using moviesapi.Interfaces;
using moviesapi.Services;
using moviesapi.Utilities;
using moviesapi.Models.Dto;

public class UserProcess :IUserProcess
{
    private readonly CosmosDbService service;
    private readonly Container container;
    private readonly Utilities utilities;
    public UserProcess(CosmosDbService _service, Utilities _utilities)
    {
        service = _service;
        container = service.GetContainerInstance<Models.User>();
        utilities = _utilities;
    }

    public async Task<ItemResponse<ReviewDto>> postUserReview(ReviewDto userReview)
    {
        var response = await container.CreateItemAsync(userReview, new PartitionKey(Convert.ToString(userReview.UserId)));
        return response;
    }

    public async Task<(List<ReviewDto>, string continuationToken)> GetUserReviews(int pageSize,Guid userId, string continuationToken)
    {
        var result = new List<ReviewDto>();
        var token = utilities.Decode(continuationToken);
        var query = new QueryDefinition("SELECT c.id, c.userId, c.reviewId, c.rating, c.review, c.reviewDate, c.movieId from c WHERE c.entityType=@entityType AND c.userId=@userId")
                    .WithParameter("@entityType","review")
                    .WithParameter("@userId", Convert.ToString(userId));

        var opts = new QueryRequestOptions
        {
            MaxItemCount = pageSize,
            PartitionKey = new PartitionKey(Convert.ToString(userId))
        };
        var iterator = container.GetItemQueryIterator<ReviewDto>(query,token,opts);
        var response = await iterator.ReadNextAsync();
        result.AddRange(response.Resource);

        string continuationTokenEncoded = utilities.Encode(response.ContinuationToken);
        return (result, continuationTokenEncoded);
    }
}