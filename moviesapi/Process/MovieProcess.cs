using System;
using moviesapi.Services;
using moviesapi.Models;

namespace moviesapi.Process
{
    public class MovieProcess : BaseProcess<Movie>
    {
        public MovieProcess(CosmosDbService service) : base(service)
        {}

        public override async Task<List<Movie>> GetAllItemsAsync(string userId)
        {
            var result = new List<Movie>();
            try
            {
                var query = cosmosService.container.GetItemQueryIterator<Movie>("SELECT * FROM c");                
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
}