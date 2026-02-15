namespace moviesapi.Process;
using moviesapi.Interfaces;
using moviesapi.Services;
using moviesapi.Process;
using moviesapi.Utilities;

public class UnitOfWork : IUnitOfWork
{
    /*private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _cosmosDbService?.Dispose();
            }
            _disposed = true;
        }
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }*/

    private IReviewProcess _reviewProcess;
    private IMovieProcess _movieProcess;
    private IUserProcess _userProcess;
    private readonly CosmosDbService _cosmosDbService;
    private readonly Utilities _utilities;

    public UnitOfWork(CosmosDbService cosmosDbService, Utilities utilities)
    {
        _cosmosDbService = cosmosDbService;
        _utilities = utilities;
    }

    public IReviewProcess ReviewProcess => _reviewProcess ??= new ReviewProcess(_cosmosDbService);
    public IMovieProcess MovieProcess => _movieProcess ??= new MovieProcess(_cosmosDbService, _utilities);
    public IUserProcess UserProcess => _userProcess ??= new UserProcess(_cosmosDbService);
}
