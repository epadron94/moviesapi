using System;
using Microsoft.Azure.Cosmos;
using moviesapi.Interfaces;
using moviesapi.Services;

namespace moviesapi.Process
{
    public class BaseProcess<T>: IProcess<T>
    {
        public CosmosDbService cosmosService;

        public BaseProcess(CosmosDbService service)
        {
            cosmosService = service;
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

        public virtual async Task<List<T>> GetAllItemsAsync(string userId)
        {
            throw new NotImplementedException();
        }      
    }    
}
