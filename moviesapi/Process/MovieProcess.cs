namespace moviesapi.Process;
using System;
using moviesapi.Services;
using moviesapi.Models;
using moviesapi.Interfaces;
using System.ComponentModel;
using _cosmos=Microsoft.Azure.Cosmos;

public class MovieProcess : IProcess<Movie>//BaseProcess<Movie>
{
    private readonly CosmosDbService cosmosService;

    private readonly _cosmos.Container container;

    public MovieProcess(CosmosDbService service)
    {
        cosmosService = service;
        container = cosmosService.GetContainerInstance<Movie>();
    }

    public async Task<Movie> GetItemAsync(string id, string partitionKey)
    {
        throw new NotImplementedException();
    }

    public  async Task<Movie> AddItemAsync(Movie item)
    {
        throw new NotImplementedException();
    }
    public async Task<Movie> UpdateItemAsync(string id, Movie item)
    {
        throw new NotImplementedException();
    }

    public Task DeleteItemAsync(string id, string partitionKey)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Movie>> GetAllItemsAsync(string userId)
    {
        var result = new List<Movie>();
        try
        {
            var query = container.GetItemQueryIterator<Movie>("SELECT * FROM c");                
            while(query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                result.AddRange(response);
            }                
        }
        catch(Exception ex) 
        {
            throw ex;
        }
        return result;
    }
    

}
