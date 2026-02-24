namespace moviesapi.Models.Responses;
using moviesapi.Models.Dto;
using Microsoft.Azure.Cosmos;

public class ReviewResponse
{
    private ItemResponse<ReviewDto> _Review;
    private CosmosException Exception;

    public ReviewResponse(ItemResponse<ReviewDto> review)
    {
        _Review = review;
    }
    public ReviewResponse(CosmosException exception)
    {
        Exception = exception;
    }

    public ItemResponse<ReviewDto> GetReview => _Review;
    public CosmosException GetException => Exception;
}