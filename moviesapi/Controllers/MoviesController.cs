namespace moviesapi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moviesapi.Process;
using moviesapi.Services;
using moviesapi.Models;
using moviesapi.Interfaces;
using moviesapi.Process;
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly MovieProcess process;

    public MoviesController(MovieProcess movieProcess)
    {
        process = movieProcess;
    }

    [Authorize]
    [HttpGet("api/HelloWorld")]
    public IActionResult HelloWorld()
    {
        //var result = process.GetAllItemsAsync("anyuser");
        return  Ok("moshi moshi from movies api");
    }

}