namespace moviesapi.Models;
using moviesapi.Interfaces;
public class MovieGenre : IModel
{
    public string id {get;set;}
    public string genre {get;set;}
    public string movieId {get;set;} //partition key
}