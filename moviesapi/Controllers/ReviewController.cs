namespace moviesapi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moviesapi.Services;
using _cosmos=Microsoft.Azure.Cosmos;
using moviesapi.Process;

[ApiController]
public class ReviewController : ControllerBase
{
    private readonly ReviewProcess process;

    public ReviewController(ReviewProcess reviewProcess)
    {
        process = reviewProcess;
    }

    [Authorize]
    [HttpGet("api/reviews/reviewsBymovie")]
    public async Task<IActionResult> GetReviewsByMovieId([FromQuery]string movieId)
    {
          return Ok();   
    }

}