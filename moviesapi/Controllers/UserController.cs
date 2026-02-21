using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moviesapi.Models.Dto;
using moviesapi.Services;

namespace moviesapi.Controllers;

public class UserController : ControllerBase
{
    private readonly UserService service;

    public UserController(UserService _service)
    {
        service = _service;    
    }

    [Authorize]
    [HttpGet("api/user/reviews")]
    public async Task<IActionResult> GetUserReviews([FromQuery, Required, Range(2,10)]int pageSize, string continuationToken=null)
    {
        string userObjectId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        Guid userId = new Guid(userObjectId);
        var(response, nextToken, requestCharge) = await service.GetUserReviews(pageSize, userId,continuationToken);
        return Ok(new {response, nextToken, requestCharge});
    }

    

}