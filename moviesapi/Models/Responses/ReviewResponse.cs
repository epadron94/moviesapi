namespace moviesapi.Models.Responses;
using moviesapi.Models.Dto;
using Microsoft.Azure.Cosmos;

public class ReviewResponse
{
    private ItemResponse<ReviewDto> _Review;
    private CosmosException Exception;
    public bool IsError {get;}

    public ReviewResponse(ItemResponse<ReviewDto> review)
    {
        _Review = review;
        IsError = false;
    }
    public ReviewResponse(CosmosException exception)
    {
        Exception = exception;
        IsError = true;
    }
    public ReviewResponse()
    {
        IsError = false;
    }

    [Obsolete]
    public ItemResponse<ReviewDto> GetReview => _Review;
    public ReviewDto GetReviewDto => _Review.Resource; 
    public CosmosException GetException => Exception;
    public double GetRequestCharge => _Review.RequestCharge;
}