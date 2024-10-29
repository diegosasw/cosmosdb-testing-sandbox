using System.Net;
using Microsoft.Azure.Cosmos;

// ReSharper disable ConvertToPrimaryConstructor

namespace Sample.Api.CosmosDb;

public class CosmosService
{
    private readonly CosmosClient _cosmosClient;

    public CosmosService(CosmosClient cosmosClient)
        => _cosmosClient = cosmosClient;
    
    public async Task<CosmosOperationResult> InsertDocumentAsync(string databaseName, string containerName, CosmosDbDocument document, CancellationToken cancellationToken)
    {
        try
        {
            var container = _cosmosClient.GetDatabase(databaseName).GetContainer(containerName);
            var itemResponse = await container.CreateItemAsync(
                item: document,
                partitionKey: new PartitionKey(document.id),
                cancellationToken: cancellationToken);
            return itemResponse.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created 
                ? CosmosOperationResult.Success() 
                : CosmosOperationResult.Error($"Failed to insert document {document} in {containerName}. Error code {itemResponse.StatusCode}");
        }
        catch (Exception exception)
        {
            return CosmosOperationResult.Error($"Unexpected error inserting document {document} in {containerName}. {exception.Message}");
        }
    }
}

