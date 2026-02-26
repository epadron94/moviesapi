namespace moviesapi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moviesapi.Services;
using System.Security.Claims;
using moviesapi.Models.Dto;

[ApiController]
public class ReviewController : ControllerBase
{
    private readonly ReviewService service;

    public ReviewController(ReviewService _service)
    {
        service = _service;
    }

    [Authorize]
    [HttpPost("api/reviews/postReview")]
    public async Task<IActionResult> PostReview([FromBody]ReviewDto review)
    {

        string userObjectId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        review.UserId= new Guid(userObjectId);
        var (response, requestCharge) = await service.postReview(review);
        Console.WriteLine("Post review request charge: " + requestCharge);
        return Ok(requestCharge);
    }

    [Authorize]
    [HttpPatch("api/reviews/patchReview")]
    public async Task<IActionResult> PatchReview([FromBody]ReviewDto review)
    {
        if(!review.Id.Equals(review.ReviewId))
            throw new ArgumentException("reviewId mismatched");
        
        string userObjectId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        review.UserId= new Guid(userObjectId);
        
        var (response, requestCharge) = await service.PatchReview(review);
        Console.WriteLine("PatchReview requestCharge: " + requestCharge.ToString());
        
        return Ok(requestCharge);
        

    }
    
    [Authorize]
    [HttpDelete("api/reviews/deleteReview")]
    public async Task<IActionResult> DeleteReview([FromQuery]Guid reviewId,[FromQuery] Guid movieId)
    {
        string userObjectId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        Guid userId = new Guid(userObjectId);
        var response = await service.DeleteReview(reviewId, movieId, userId);
        return NoContent();
    }


}