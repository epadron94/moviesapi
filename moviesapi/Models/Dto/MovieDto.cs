namespace moviesapi.Models.Dto;
using System;

public class MovieDto
{
    public string id {get;set;} //partiton key
    public string title {get;set;}
    public int releaseYear {get;set;}
    public DateTime releaseDate {get;set;}
    public string plot {get;set;}
    public int rating {get;set;}
    public int runtimeSeconds {get;set;}   
}