namespace moviesapi.Models.Dto;
using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class MovieDto
{
    [JsonProperty("id")]
    public string Id {get;set;} 
    [JsonProperty("title")]
    public string Title {get;set;}
    [JsonProperty("releaseYear")]
    public int ReleaseYear {get;set;}
    [JsonProperty("releaseDate")]
    public DateTime ReleaseDate {get;set;}
    [JsonProperty("plot")]
    public string Plot {get;set;}
    [JsonProperty("rating")]
    [Range(0,10)]
    public int Rating {get;set;}
    [JsonProperty("runtimeMin")]
    public int RuntimeMin {get;set;}   
}