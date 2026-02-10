namespace moviesapi.Models;
using System;
using moviesapi.Interfaces;

public class Review : IModel
{
    public string id {get;set;} //partition key
    public string movieId {get;set;}
    public string userId {get;set;} 
    public int rating {get;set;}
    public string review {get;set;}
    public DateTime reviewDate {get;set;}
}