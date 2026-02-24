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
using Microsoft.Azure.Cosmos;

public class MovieProcess : IMovieProcess//BaseProcess<Movie>
{
    private readonly CosmosDbService service;

    private readonly _cosmos.Container container;
    private readonly Utilities utilities;

    public MovieProcess(CosmosDbService _service, Utilities _utilities)
    {
        service = _service;
        container = service.GetContainerInstance<Movie>();
        utilities = _utilities;
    }

    public async Task<(List<MovieDto>, string ContinuationToken, double cost)> GetAllItemsAsync(int? pageSize, string continuationToken)
    {
        var result = new List<MovieDto>();
        try
        {
            var token = utilities.Decode(continuationToken);
            var query = new QueryDefinition("SELECT c.id, c.title, c.releaseYear, c.releaseDate, c.plot, c.rating, c.runtimeMin FROM c WHERE c.entityType=@entityType ORDER BY c.releaseDate DESC")
                                            .WithParameter("@entityType","movie");
            var requestOptions = new _cosmos.QueryRequestOptions
            {
                MaxItemCount = pageSize
            };
            var iterator = container.GetItemQueryIterator<MovieDto>(query,token, requestOptions);

            var response = await iterator.ReadNextAsync();    
            result.AddRange(response.Resource);            
            
            string continuationTokenEncoded = null;

            if(iterator.HasMoreResults)
            {
                var checkForMoreRows = await  iterator.ReadNextAsync();

                if(iterator.HasMoreResults)
                    continuationTokenEncoded = utilities.Encode(response.ContinuationToken);
            }

            return(result, continuationTokenEncoded, response.RequestCharge);

        }
        catch(_cosmos.CosmosException ex)when(ex.StatusCode ==  System.Net.HttpStatusCode.TooManyRequests)
        {
            Console.WriteLine("Request was throttled. Retry after: " + ex.RetryAfter);
            throw ex;
        }
    }

    public async Task<(MovieDto, double)> GetMovieById(string id)
    {
        var result = new MovieDto();
        var response = await container.ReadItemAsync<MovieDto>(id,new _cosmos.PartitionKey(id));
        result = response.Resource;

        return (result, response.RequestCharge);
    }
    public async Task<bool> ItemExistsAsync(Guid Id)
    {
        string strId = Convert.ToString(Id);
        try
        {
            var response = await container.ReadItemStreamAsync(strId, new _cosmos.PartitionKey(strId));
            return response.IsSuccessStatusCode;
        }
        catch(_cosmos.CosmosException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<ItemResponse<ReviewDto>> PostMovieReview(ReviewDto review)
    {
        var response = await container.CreateItemAsync(review, new _cosmos.PartitionKey(Convert.ToString(review.MovieId)));
        return response;
    }


    public async Task<ItemResponse<ReviewDto>> PatchMovieReviewAsync(ReviewDto review)
    {
        //var response = await container.ReplaceItemAsync(review,review.MovieId.ToString(), new _cosmos.PartitionKey(review.MovieId.ToString()));
        var patchOp = new List<_cosmos.PatchOperation>
        {
            _cosmos.PatchOperation.Replace("/rating", review.Rating),
            _cosmos.PatchOperation.Replace("/review",review.Review),
            _cosmos.PatchOperation.Replace("/reviewDate", DateTime.UtcNow)
        };

        var response = await container.PatchItemAsync<ReviewDto>(review.Id.ToString(),
                                                                 new PartitionKey(review.MovieId.ToString()),
                                                                 patchOp); 
        return response;
    }
}


