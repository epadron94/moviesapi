namespace moviesapi.Process;
using Microsoft.Azure.Cosmos;
using moviesapi.Interfaces;
using moviesapi.Services;
using moviesapi.Models;
using moviesapi.Models.Dto;

public class UserProcess :IUserProcess
{
    private readonly CosmosDbService service;
    private readonly Container container;

    public UserProcess(CosmosDbService _service)
    {
        service = _service;
        container = service.GetContainerInstance<Models.User>();
    }

    public async Task<ItemResponse<ReviewDto>> postUserReview(ReviewDto userReview)
    {
        var response = await container.CreateItemAsync(userReview, new PartitionKey(Convert.ToString(userReview.UserId)));
        return response;
    }
}