namespace moviesapi.Models;
using moviesapi.Interfaces;


public class User : IModel
{
    public string id {get;set;} // partition key
    public string tenantId {get;set;}
    public string displayName {get;set;}
}