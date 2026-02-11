namespace moviesapi.Utilities;

using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger)
    {
        logger = _logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception ex, CancellationToken cancellationToken)
    {
        var problem = ex switch
        {
            ArgumentNullException argEx => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = argEx.Message,
                Instance = httpContext.Request.Path
            },
            ArgumentException argEx => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = argEx.Message,
                Instance = httpContext.Request.Path
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "You're trying to access a protected endpoint",
                Instance = httpContext.Request.Path
            },
            CosmosException cosmosEx => cosmosEx.StatusCode switch
                {
                    HttpStatusCode.NotFound => new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Resource Not Found",
                        Detail = "The item requested was not found in Database",
                        Instance = httpContext.Request.Path
                    },
                    HttpStatusCode.RequestTimeout => new ProblemDetails
                    {
                        Status = StatusCodes.Status408RequestTimeout,
                        Title = "Request Timeout",
                        Detail = "The operation did not complete within the allotted amount of time",
                        Instance = httpContext.Request.Path
                    },
                    HttpStatusCode.Conflict => new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Conflict",
                        Detail = "The ID provided for operation has been taken by an existig resource",
                        Instance = httpContext.Request.Path
                    },
                    HttpStatusCode.TooManyRequests => new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Too Many Requests",
                        Detail = "RU provisioned limit exceeded",
                        Instance = httpContext.Request.Path
                    },
                    HttpStatusCode.BadRequest => cosmosEx.SubStatusCode switch
                        {
                            20007 => new ProblemDetails
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Title = "Bad Request",
                                Detail = "Continuation token malformed",
                                Instance = httpContext.Request.Path
                            }
                        }

                }, 
            _=> new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = "Something wrong ocurred trying to process your request",
                Instance = httpContext.Request.Path
            }

        };
        
        httpContext.Response.StatusCode = problem.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;

    }

}       