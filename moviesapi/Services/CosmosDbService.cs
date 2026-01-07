using Microsoft.Azure.Cosmos;

namespace moviesapi.Services
{
    public class CosmosDbService
    {
        public CosmosClient cosmosClient;
        public Container container;
        public CosmosDbService(CosmosClient _cosmosClient, IConfiguration _configuration, string containerName)
        {
            cosmosClient = _cosmosClient;
            container = cosmosClient.GetContainer(_configuration["CosmosDb:databaseName"],containerName);
        }
    }   
}   