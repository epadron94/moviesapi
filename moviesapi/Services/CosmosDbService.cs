namespace moviesapi.Services;
using System.Collections.Generic;
using _cosmos=Microsoft.Azure.Cosmos; 
using moviesapi.Interfaces;   
using moviesapi.Models;
public class CosmosDbService
{
    public _cosmos.CosmosClient cosmosClient;

    public _cosmos.Database database;
    string databaseName;
    //public _cosmos.Container container;
    public CosmosDbService(_cosmos.CosmosClient _cosmosClient, IConfiguration _configuration)
    {
        cosmosClient = _cosmosClient;
        database = cosmosClient.GetDatabase(_configuration["CosmosDb:DatabaseName"]);
        databaseName = _configuration["CosmosDb:DatabaseName"];
        //container = GetContainerInstance<_cosmos.Container,T>();
    }

    public  _cosmos.Container GetContainerInstance<T>() where T : IModel
    {
        switch(typeof(T).Name)
        {
            case "Movie":
                return cosmosClient.GetContainer(databaseName, typeof(Movie).Name);
            default:
                throw new ArgumentException("Invalid type");
        }
    }
}   
