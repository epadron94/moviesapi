namespace moviesapi.Models.Dto;
using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class ReviewDto
{
    [JsonProperty("id")]
    public Guid Id {get;set;} //partition key

    [JsonProperty("userId")]
    public Guid UserId {get;set;} 

    [JsonProperty("entityType")]
    public string EntityType {get;}= "review";

    [JsonProperty("reviewId")]
    public Guid ReviewId {get;set;}

    [JsonRequired]
    [JsonProperty("movieId")]
    public Guid MovieId {get;set;}

    [JsonRequired]
    [JsonProperty("rating")]
    [Range(1,5)]
    public int Rating {get;set;}

    [JsonRequired]
    [JsonProperty("review")]
    public string Review {get;set;}

    [JsonProperty("reviewDate")]
    public DateTime ReviewDate {get;set;}

    public ReviewDto()
    {
        Id = Guid.NewGuid();
        ReviewId = Id;
        ReviewDate = DateTime.Now;
    }
}