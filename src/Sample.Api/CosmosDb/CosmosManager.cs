using System.Net;
using Microsoft.Azure.Cosmos;
// ReSharper disable ConvertToPrimaryConstructor

namespace Sample.Api.CosmosDb;

public class CosmosManager
{
    private readonly CosmosClient _cosmosClient;

    public CosmosManager(CosmosClient cosmosClient)
        => _cosmosClient = cosmosClient;
    
    public async Task<CosmosOperationResult> Create(string databaseName)
    {
        var result = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
        return result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created 
            ? CosmosOperationResult.Success() 
            : CosmosOperationResult.Error($"Failed to create database {databaseName}. Error code {result.StatusCode}");
    }
    
    public async Task<CosmosOperationResult> CreateContainer(string databaseName, string containerName)
    {
        var databaseResult = await Create(databaseName);
        if (!databaseResult.IsSuccessful)
        {
            return CosmosOperationResult.Error($"Failed to create database {databaseName} during creation of container {containerName}. {databaseResult.ErrorMessage}");
        }
        var result = await _cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id");
        
        return result.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created 
            ? CosmosOperationResult.Success() 
            : CosmosOperationResult.Error($"Failed to create container {containerName}. Error code {result.StatusCode}");
    }
    
    public async Task<CosmosOperationResult> DeleteContainer(string databaseName, string containerName)
    {
        var result = await _cosmosClient.GetDatabase(databaseName).GetContainer(containerName).DeleteContainerAsync();
        
        return result.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent
            ? CosmosOperationResult.Success() 
            : CosmosOperationResult.Error($"Failed to delete container {containerName} in database {databaseName}. Error code {result.StatusCode}");
    }
    
    public async Task<CosmosOperationResult> ClearContainerAsync(string databaseName, string containerName)
    {
        try
        {
            var container = _cosmosClient.GetDatabase(databaseName).GetContainer(containerName);
            var query = container.GetItemQueryIterator<dynamic>("SELECT c.id, c.partitionKey FROM c");

            while (query.HasMoreResults)
            {
                var items = await query.ReadNextAsync();
                var tasks = new List<Task>();

                foreach (var item in items)
                {
                    var partitionKey = new PartitionKey(item.partitionKey);
                    tasks.Add(container.DeleteItemAsync<dynamic>(item.id.ToString(), partitionKey));
                }

                await Task.WhenAll(tasks);
            }

            return CosmosOperationResult.Success();
        }
        catch (Exception exception)
        {
            return CosmosOperationResult.Error($"Unexpected error clearing container {containerName} in database {databaseName}. {exception.Message}");
        }
    }
}