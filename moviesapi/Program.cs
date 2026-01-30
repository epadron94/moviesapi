using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using System.Reflection;
using moviesapi.Models;
using moviesapi.Process;
using moviesapi.Services;


System.Threading.Thread.Sleep(10000);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddControllers();
//Keyvault app identity access
var keyVaultUriConfig = builder.Configuration.GetSection("KeyVault");
Console.WriteLine("KeyVault URI: " + keyVaultUriConfig["Uri"]);
builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUriConfig["Uri"]),
    new DefaultAzureCredential()
);
/*Console.WriteLine("CosmosDbAccount" + builder.Configuration["CosmosDbAccount"]);
Console.WriteLine("primaryMasterKey" + builder.Configuration["primaryMasterKey"]);*/
//get secret value using builder.Configuration["secretname"]

//CosmosDb connection using singleton
var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddSingleton<CosmosClient>(t =>
{
   var clientOptions = new CosmosClientOptions
   {
       ApplicationPreferredRegions = cosmosDbConfig.GetSection("PreferredRegions").Get<List<string>>()
   };
   Console.WriteLine("CosmosDbAccount: " + builder.Configuration["CosmosDbAccount"]);
   Console.WriteLine("primaryMasterKey: " + builder.Configuration["primaryMasterKey"]);
   return new CosmosClient(
    accountEndpoint: builder.Configuration["CosmosDbAccount"],
    authKeyOrResourceToken: builder.Configuration["primaryMasterKey"],
    clientOptions: clientOptions
   );

});
builder.Services.AddSingleton<CosmosDbService>();

builder.Services.AddSingleton(t =>
{
    var client= t.GetRequiredService<CosmosClient>();
    return client.GetContainer(builder.Configuration["CosmosDb:DatabaseName"],
                              typeof(Movie).Name
                             );
});

builder.Services.AddScoped<MovieProcess>();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();
app.Run();


//builder.Configuration["CosmosDbAccount"]
//CosmosDbKey