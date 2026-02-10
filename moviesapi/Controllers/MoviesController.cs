namespace moviesapi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moviesapi.Process;
using moviesapi.Services;
using moviesapi.Models;
using moviesapi.Interfaces;
using moviesapi.Services;
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly MovieService service;

    public MoviesController(MovieService movieService)
    {
        service = movieService;
    }

    [Authorize]
    [HttpGet("api/movies/getmovies")]
    public async Task<IActionResult> GetMovies(int pageSize, string continuationToken = null)
    {
        var (result, nextToken, cost) = await service.GetAllItemsAsync(pageSize, continuationToken);
        return  Ok(new {result, nextToken, cost});
    }

}