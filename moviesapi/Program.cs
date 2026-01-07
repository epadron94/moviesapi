using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();
builder.Services.AddControllers();

//Keyvault app identity access
var keyVaultUriConfig = builder.Configuration.GetSection("KeyVault");
builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUriConfig["Uri"]),
    new DefaultAzureCredential()
);
//get secret value using builder.Configuration["secretname"]
/*
builder.Services.AddSingleton(t =>
    {
        var cosmosClientOptions = new CosmosClientOptions
        {
            ApplicationPreferredRegions = CosmosDbConfig.GetSection(Static.PreferredRegions).Get<List<string>>()
        };
        return new CosmosClient(
            accountEndpoint:cosmosDbAccount,// CosmosDbConfig["Account"],
            authKeyOrResourceToken: cosmosDbKey,//CosmosDbConfig["Key"],
            clientOptions: cosmosClientOptions
        );
    }
);
*/
//CosmosDb connection using singleton
var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddSingleton<CosmosClient>(t =>
{
   var clientOptions = new CosmosClientOptions
   {
       ApplicationPreferredRegions = cosmosDbConfig.GetSection("PreferredRegions").Get<List<string>>()
   };
   return new CosmosClient(
    accountEndpoint: builder.Configuration["CosmosDbAccount"],
    authKeyOrResourceToken: builder.Configuration["CosmosDbKey"],
    clientOptions: clientOptions
   );

});



var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();
app.Run();


//builder.Configuration["CosmosDbAccount"]
//CosmosDbKey