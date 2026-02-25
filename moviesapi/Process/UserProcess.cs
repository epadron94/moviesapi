namespace moviesapi.Process;
using Microsoft.Azure.Cosmos;
using moviesapi.Interfaces;
using moviesapi.Services;
using moviesapi.Utilities;
using moviesapi.Models.Dto;
using moviesapi.Models.Responses;
using System.Net;

public class UserProcess : IUserProcess
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

    public async Task<ReviewResponse> postUserReview(ReviewDto userReview)
    {
        try
        {
            var response = await container.CreateItemAsync(userReview, new PartitionKey(Convert.ToString(userReview.UserId)));    
            return new ReviewResponse(response);
        }
        catch(CosmosException ex)
        {
            return new ReviewResponse(ex);
        }
    }

    public async Task<(List<ReviewDto>, string, double)> GetUserReviews(int pageSize,Guid userId, string continuationToken)
    {
        var result = new List<ReviewDto>();
        var token = utilities.Decode(continuationToken);
        var query = new QueryDefinition("SELECT c.id, c.userId, c.reviewId, c.rating, c.review, c.reviewDate, c.movieId from c WHERE c.entityType=@entityType AND c.userId=@userId ORDER BY c.reviewDate DESC")
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
        return (result, continuationTokenEncoded, response.RequestCharge);
    }

    public async Task<ReviewResponse> PatchUserReviewAsync(ReviewDto review)
    {
        try
        {
            var patchOp = new List<PatchOperation>
            {
                PatchOperation.Replace("/rating", review.Rating),
                PatchOperation.Replace("/review",review.Review),
                PatchOperation.Replace("/reviewDate", DateTime.UtcNow)
            };
            PatchItemRequestOptions opts =  new PatchItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };
            var response = await container.PatchItemAsync<ReviewDto>(review.Id.ToString(), 
                                                                    new PartitionKey(review.UserId.ToString()),
                                                                    patchOp,
                                                                    opts);    
            return new ReviewResponse(response);
        }
        catch(CosmosException ex)
        {
            return new ReviewResponse(ex);
        }
    }

    public async Task<bool> DeleteUserReview(Guid reviewId, Guid userId)
    {
        var response = await container.DeleteItemAsync<ReviewDto>(reviewId.ToString(), new PartitionKey(userId.ToString()));
        return true;
    }

    public async Task<ItemResponse<ReviewDto>> GetReview(Guid reviewId, Guid userId)
    {
        var response = await container.ReadItemAsync<ReviewDto>(reviewId.ToString(), new PartitionKey(userId.ToString()));
        return response;
    }
}