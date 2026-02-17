namespace moviesapi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moviesapi.Process;
using moviesapi.Services;
using moviesapi.Models;
using moviesapi.Interfaces;
using System.ComponentModel.DataAnnotations;
using moviesapi.Utilities;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly MovieService service;
    private readonly Utilities utilities; 

    public MoviesController(MovieService movieService, Utilities _utilities)
    {
        service = movieService;
        utilities = _utilities;
    }

    [Authorize]
    [HttpGet("api/movies/getmovies")]
    public async Task<IActionResult> GetMovies([FromQuery, Required, Range(2,25)]int? pageSize, string continuationToken = null)
    {
        if(continuationToken is not null  && !utilities.IsBase64(continuationToken)) throw new ArgumentNullException("continuationToken is not valid");
        var (result, nextToken, cost) = await service.GetAllItemsAsync(pageSize, continuationToken);
        return  Ok(new {result, nextToken, cost});
    }

    [Authorize]
    [HttpGet("api/movies/getmovie")]
    public async Task<IActionResult> GetMovie(string id)
    {
        var result = await service.GetMovieById(id);
        return Ok(result);   
    }

    [Authorize]
    [HttpGet("api/movies/reviews")]
    public async Task<ActionResult> GetMovieReviews([FromQuery, Required, Range(2,10)]int pageSize, [FromQuery, Required]Guid movieId,[FromQuery] string continuationToken = null)
    {

        if(continuationToken is not null  && !utilities.IsBase64(continuationToken)) throw new ArgumentNullException("continuationToken is not valid");
        var (result, nextToken) = await service.GetMovieReviews(pageSize,continuationToken, movieId);
        return Ok(new {result, nextToken});

    }

}