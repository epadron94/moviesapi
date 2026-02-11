namespace moviesapi.Services;
using moviesapi.Interfaces;
using System.Collections.Generic;
using moviesapi.Utilities;
using moviesapi.Models.Dto;
using System.Security.Cryptography.X509Certificates;

public class MovieService
{
    IUnitOfWork _unitOfWork;

    public MovieService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;   
    }

    public async Task<(List<MovieDto>, string ContinuationToken, double cost)> GetAllItemsAsync(int ?pageSize, string continuationToken)
    {
        //if(pageSize == 0)throw new ArgumentNullException();
        //var result =  new List<MovieDto>();
        var (_response, _continuationToken, _cost) = await _unitOfWork.MovieProcess.GetAllItemsAsync(pageSize.Value, continuationToken);
        return(_response, _continuationToken, _cost);
    }

    public async Task<MovieDto> GetMovieById(string id)
    {
        var result = await _unitOfWork.MovieProcess.GetMovieById(id);
        return result;   
    }

}