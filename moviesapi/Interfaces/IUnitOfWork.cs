namespace moviesapi.Interfaces;


public interface IUnitOfWork
{
    IReviewProcess ReviewProcess {get;}
    IMovieProcess MovieProcess {get;}     
    IUserProcess UserProcess {get;}
}