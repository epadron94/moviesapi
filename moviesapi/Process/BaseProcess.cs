/*

namespace moviesapi.Process;
using moviesapi.Services;
using System;
using System.ComponentModel;
using moviesapi.Interfaces;
using System.Collections.Generic;
using _cosmos=Microsoft.Azure.Cosmos;
[Obsolete("Use IProcess<T> implementation instead")]
public class BaseProcess<T>: IProcess<T> where T : IModel
{
    private readonly CosmosDbService<T> cosmosService;
    public _cosmos.Container container;
    public BaseProcess(CosmosDbService<T> service)
    {
        cosmosService = service;
        container = cosmosService.GetContainerInstance<_cosmos.Container,T>();
    }

    public async Task<T> GetItemAsync(string id, string partitionKey)
    {
        throw new NotImplementedException();
    }
    public async Task<T> AddItemAsync(T item)
    {
        throw new NotImplementedException();
    }
    public async Task<T> UpdateItemAsync(string id, T item)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteItemAsync(string id, string partitionKey)
    {
        throw new NotImplementedException();
    }

    public async Task<List<T>> GetAllItemsAsync(string userId)
    {
        var result = new List<T>();
        try
        {
            var query = cosmosService.container.GetItemQueryIterator<T>("SELECT * FROM c");                
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
*/