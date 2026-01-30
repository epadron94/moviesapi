namespace moviesapi.Models;
using System;
using moviesapi.Interfaces;


public class Movie : IModel
{        
    string Id {get; set;}
    public string Title {get;set;}

    public string Description {get;set;}

    public string UserId {get;set;}
        
}

