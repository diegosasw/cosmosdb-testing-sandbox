using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.CosmosDb;

// ReSharper disable ConvertToPrimaryConstructor

namespace Sample.Api;

public class CommandHandler
{
    private static readonly Random Random = new();
    
    private readonly CosmosDbSettings _cosmosDbSettings;
    private readonly CosmosService _cosmosService;

    public CommandHandler(
        CosmosDbSettings cosmosDbSettings,
        CosmosService cosmosService)
    {
        _cosmosDbSettings = cosmosDbSettings;
        _cosmosService = cosmosService;
    }
    
    public async Task<Results<Ok<CommandHttpResponse>, BadRequest<ProblemDetails>>> Handle(
        CommandHttpRequest commandHttpRequest, 
        CancellationToken cancellationToken)
    {
        
        var databaseName = _cosmosDbSettings.DatabaseName;
        var containerName = GetRandomValue(CosmosDbSettings.ContainerNames);
        var document = new CosmosDbDocument(Guid.NewGuid().ToString(), commandHttpRequest.Text, DateTime.UtcNow);
        var cosmosOperationResult = 
            await _cosmosService.InsertDocumentAsync(databaseName, containerName, document, cancellationToken);
        if (cosmosOperationResult.IsSuccessful)
        {
            return TypedResults.Ok(new CommandHttpResponse(databaseName, containerName, commandHttpRequest));
        }
        
        return TypedResults.BadRequest(new ProblemDetails
        {
            Title = "Error handling command",
            Instance = ToString(),
            Status = 400,
            Detail = cosmosOperationResult.ErrorMessage
        });
    
    }
    
    private static string GetRandomValue(IEnumerable<string> containers)
    {
        var itemList = containers.ToList();
        var randomIndex = Random.Next(itemList.Count);
        return itemList[randomIndex];
    }
}