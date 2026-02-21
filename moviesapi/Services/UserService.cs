using moviesapi.Interfaces;
using moviesapi.Models.Dto;

namespace moviesapi.Services;

public class UserService
{
    private IUnitOfWork unitOfWork;
    public UserService(IUnitOfWork _unitOfWork)
    {
        unitOfWork = _unitOfWork;
    }

    public async Task<(List<ReviewDto>, string , double)> GetUserReviews(int pageSize, Guid userId, string continuationToken = null)
    {
        var (_response, _continuationToken, requestCharge ) = await unitOfWork.UserProcess.GetUserReviews(pageSize, userId, continuationToken);
        return (_response, _continuationToken, requestCharge);
        
    }

}
