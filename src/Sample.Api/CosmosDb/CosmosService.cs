using System.Net;
using Microsoft.Azure.Cosmos;

// ReSharper disable ConvertToPrimaryConstructor

namespace Sample.Api.CosmosDb;

public class CosmosService
{
    private readonly CosmosClient _cosmosClient;

    public CosmosService(CosmosClient cosmosClient)
        => _cosmosClient = cosmosClient;
    
    public async Task<CosmosOperationResult> InsertDocumentAsync<T>(string databaseName, string containerName, T document, CancellationToken cancellationToken)
    {
        try
        {
            var container = _cosmosClient.GetDatabase(databaseName).GetContainer(containerName);
            var result = await container.CreateItemAsync(document, cancellationToken: cancellationToken);
            return result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created 
                ? CosmosOperationResult.Success() 
                : CosmosOperationResult.Error($"Failed to insert document {document} in {containerName}. Error code {result.StatusCode}");
        }
        catch (Exception exception)
        {
            return CosmosOperationResult.Error($"Unexpected error inserting document {document} in {containerName}. {exception.Message}");
        }
    }
}

