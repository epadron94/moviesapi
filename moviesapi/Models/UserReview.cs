namespace moviesapi.Models;
using moviesapi.Interfaces;

public class UserReview : IModel
{
    public string id {get;set;}
    public string userId {get;set;} //partition key
    public string reviewId {get; set;}

}