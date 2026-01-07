using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moviesapi.Process;
using moviesapi.Services;
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly MovieProcess process;

    public MoviesController(CosmosDbService _service)
    {
        service
    }

    [Authorize]
    [HttpGet("api/HelloWorld")]
    public IActionResult HelloWorld()
    {
        return  Ok("Hello, World!");
    }

}