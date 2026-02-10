namespace moviesapi.Services;
using moviesapi.Interfaces;
using System.Collections.Generic;
using moviesapi.Utilities;
using moviesapi.Models.Dto;
public class MovieService
{
    IUnitOfWork _unitOfWork;

    public MovieService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;   
    }

    public async Task<(List<MovieDto>, string ContinuationToken, double cost)> GetAllItemsAsync(int pageSize, string continuationToken)
    {
        //var result =  new List<MovieDto>();
        var (_response, _continuationToken, _cost) = await _unitOfWork.MovieProcess.GetAllItemsAsync(pageSize, continuationToken);
        return(_response, _continuationToken, _cost);
    }

}