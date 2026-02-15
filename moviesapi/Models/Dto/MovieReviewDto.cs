namespace moviesapi.Models.Dto;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MovieReviewDto
{   
    [JsonProperty("id")]
    private string Id {get;set;}

    [JsonProperty("MovieId")]
    private string MovieId { get; set; } 
    
    [JsonProperty("Reviews")]
    public List<ReviewDto> reviews { get;set;}
}