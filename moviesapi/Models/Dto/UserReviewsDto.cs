namespace moviesapi.Models.Dto;
using System;
using System.Collections.Generic;

public class UserReviewsDto
{
    public string id {get;set;}
    public string userId { get; set; }
    public List<ReviewDto> reviews { get;set;}

}