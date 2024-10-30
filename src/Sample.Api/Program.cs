using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Sample.Api;
using Sample.Api.CosmosDb;

var builder = WebApplication.CreateBuilder(args);

// Register Services
builder
    .Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSingleton(sp =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var cosmosDbSettings = new CosmosDbSettings();
        configuration.Bind("CosmosDb", cosmosDbSettings);
        return cosmosDbSettings;
    })
    .AddSingleton(sp =>
    {
        var cosmosDbSettings = sp.GetRequiredService<CosmosDbSettings>();
        var cosmosClientOptions =
            new CosmosClientOptions
            {
                // Only needed if SSL not installed
                // HttpClientFactory = () =>
                // {
                //     var httpMessageHandler =
                //         new HttpClientHandler
                //         {
                //             ServerCertificateCustomValidationCallback =
                //                 HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                //         };
                //
                //     return new HttpClient(httpMessageHandler);
                // },
                ConnectionMode = ConnectionMode.Gateway
            };
        
        var cosmosClient =
            new CosmosClient(
                cosmosDbSettings.AccountEndpoint,
                cosmosDbSettings.PrimaryKey,
                cosmosClientOptions);
        
        return cosmosClient;
    })
    .AddScoped<CosmosService>()
    .AddScoped<CommandHandler>()
    .AddSingleton<CosmosManager>();


var app = builder.Build();

await AutoProvisionCosmosDb(app.Services);

app.UseSwagger();
app.UseSwaggerUI();

app
    .MapPost("/runsample", async (
            [FromBody] CommandHttpRequest commandHttpRequest,
            [FromServices] CommandHandler commandHandler,
            CancellationToken cancellationToken)
        => await commandHandler.Handle(commandHttpRequest, cancellationToken))
    .AllowAnonymous();


app.Run();
return;

async Task AutoProvisionCosmosDb(IServiceProvider serviceProvider)
{
    // CosmosDB Auto-provisioning
    var cosmosDbManager = serviceProvider.GetRequiredService<CosmosManager>();
    var cosmosDbSettings = serviceProvider.GetRequiredService<CosmosDbSettings>();
    var createResult = await cosmosDbManager.Create(cosmosDbSettings.DatabaseName);
    if (!createResult.IsSuccessful)
    {
        throw new Exception(createResult.ErrorMessage);
    }

    foreach (var containerName in CosmosDbSettings.ContainerNames)
    {
        var createContainerResult = await cosmosDbManager.CreateContainer(cosmosDbSettings.DatabaseName, containerName);
        if (!createContainerResult.IsSuccessful)
        {
            throw new Exception(createContainerResult.ErrorMessage);
        }
    }
}

// Explicit namespace and partial class Program to reference from tests
namespace Sample.Api
{
    // ReSharper disable once PartialTypeWithSinglePart
    public abstract partial class Program;
}