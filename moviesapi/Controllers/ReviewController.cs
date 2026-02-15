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
        var response = await service.postReview(review);
        return Ok();
    }

    [Authorize]
    [HttpGet("api/reviews/reviewsBymovie")]
    public async Task<IActionResult> GetReviewsByMovieId([FromQuery]string movieId)
    {
          return Ok();   
    }

}