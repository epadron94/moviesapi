namespace moviesapi.Process;
using System;
using moviesapi.Services;
using moviesapi.Models;
using moviesapi.Models.Dto;
using moviesapi.Interfaces;
using System.ComponentModel;
using _cosmos=Microsoft.Azure.Cosmos;
using System.Net;
using System.Net.Http.Headers;
using moviesapi.Utilities;

public class MovieProcess : IMovieProcess//BaseProcess<Movie>
{
    private readonly CosmosDbService cosmosService;

    private readonly _cosmos.Container container;

    private readonly Utilities utilities;

    public MovieProcess(CosmosDbService service, Utilities utilities)
    {
        cosmosService = service;
        container = cosmosService.GetContainerInstance<Movie>();
        this.utilities = utilities;
    }


    public async Task<(List<MovieDto>, string ContinuationToken, double cost)> GetAllItemsAsync(int pageSize, string continuationToken)
    {
        var result = new List<MovieDto>();
        try
        {
            var token = utilities.Decode(continuationToken);
            var query = "SELECT c.id, c.title, c.releaseYear, c.releaseDate, c.plot, c.rating, c.runtimeSeconds FROM c ORDER BY c.releaseDate DESC";
            var requestOptions = new _cosmos.QueryRequestOptions
            {
                MaxItemCount = pageSize,
                //PartitionKey = new _cosmos.PartitionKey("id")
            };
            var iterator = container.GetItemQueryIterator<MovieDto>(query,token,requestOptions);

            var response = await iterator.ReadNextAsync();    
            result.AddRange(response.Resource);            
            
            var continuationTokenEncoded = utilities.Encode(response.ContinuationToken);
            return(result, continuationTokenEncoded, response.RequestCharge);

        }
        catch(_cosmos.CosmosException ex)when(ex.StatusCode ==  System.Net.HttpStatusCode.TooManyRequests)
        {
            Console.WriteLine("Request was throttled. Retry after: " + ex.RetryAfter);
            throw ex;
        }
    }

  

}
