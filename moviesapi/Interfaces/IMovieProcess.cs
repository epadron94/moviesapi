namespace moviesapi.Interfaces;
using moviesapi.Models.Dto;

public interface IMovieProcess
{
    Task<(List<MovieDto>, string ContinuationToken, double cost)> GetAllItemsAsync(int ? pageSize, string continuationToken);
    Task<MovieDto> GetMovieById(string id);
}
