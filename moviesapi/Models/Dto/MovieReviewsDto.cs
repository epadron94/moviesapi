namespace moviesapi.Models.Dto;
using System;
using System.Collections.Generic;

public class MovieReviewsDto
{   
    public string id {get;set;}
    public string movieId { get; set; } 
    public List<ReviewDto> reviews { get;set;}
}