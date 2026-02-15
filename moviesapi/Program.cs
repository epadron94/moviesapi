using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using System.Reflection;
using moviesapi.Models;
using moviesapi.Process;
using moviesapi.Services;
using System.Security.Cryptography.X509Certificates;
using moviesapi.Interfaces;

using moviesapi.Utilities;

//System.Threading.Thread.Sleep(20000);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var keyVaultUriConfig = builder.Configuration.GetSection("KeyVault");
//Console.WriteLine("KeyVault URI: " + keyVaultUriConfig["Uri"]);
try
{
    var certBase64 = Environment.GetEnvironmentVariable("movieswebapicert");
    //var credential = new DefaultAzureCredential();
    if(!string.IsNullOrEmpty(certBase64))
    {
        Console.WriteLine("certificate found as env variable, proceed to authenticate to service principal using certificate");
        var bytes = Convert.FromBase64String(certBase64);
        var cert = new X509Certificate2(bytes);
        
        var spConfig= builder.Configuration.GetSection("ServicePrincipal");
        var clientId = spConfig["ClientId"];
        var tenantId = spConfig["TenantId"];

        var credential = new ClientCertificateCredential(tenantId, clientId, cert);
        
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUriConfig["Uri"]),
            credential
        );
    }
    else
    {
        Console.WriteLine("no certificate found, proceed to authenticate with current az cli user logged");
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUriConfig["Uri"]),
            new DefaultAzureCredential()
        );        
    }
}
catch(Exception ex)
{
    Console.WriteLine("Something went wrong loading the certificate: inner exception:" + ex.InnerException + " message: " + ex.Message);
}
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
builder.Services.AddSingleton(t=>
{
    var client= t.GetRequiredService<CosmosClient>();
    return client.GetContainer(builder.Configuration["CosmosDb:DatabaseName"],
                              typeof(Review).Name
                             );
});
builder.Services.AddSingleton(t =>
{
    var client = t.GetRequiredService<CosmosClient>();
    return client.GetContainer(builder.Configuration["CosmosDb:DatabaseName"],
                                typeof(moviesapi.Models.User).Name);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(t =>
{
    var cosmosDbService =  t.GetRequiredService<CosmosDbService>();
    var utilities = t.GetRequiredService<Utilities>();
    return new UnitOfWork(cosmosDbService, utilities);
});

builder.Services.AddScoped<MovieService>(l =>
{
    var unitOfWork =  l.GetRequiredService<IUnitOfWork>();
    return new MovieService(unitOfWork);
});
builder.Services.AddScoped<ReviewService>(l=>
{
    var unitOfWork = l.GetRequiredService<IUnitOfWork>();
    return new ReviewService(unitOfWork);
});




/*builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();*/

//builder.Services.AddScoped<MovieService>();
builder.Services.AddSingleton<Utilities>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();
app.Run();
