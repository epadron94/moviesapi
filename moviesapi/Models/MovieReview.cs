namespace moviesapi.Models;
using moviesapi.Interfaces;

public class MovieReview : IModel
{
    public string id {get;set;}
    public string movieId {get;set;} //partition key
    public string reviewId {get;set;}
}