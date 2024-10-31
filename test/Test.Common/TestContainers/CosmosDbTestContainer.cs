using Testcontainers.CosmosDb;

namespace Test.Common.TestContainers;

public class CosmosDbTestContainer
{
    private CosmosDbContainer? _cosmosDbContainer;

    public async Task<CosmosDbTestContainerResult> Start()
    {
        _cosmosDbContainer = 
            new CosmosDbBuilder()
                .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE","127.0.0.1")
                .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "false")
                .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "12")
                .Build();
        await _cosmosDbContainer.StartAsync();

        var result =
            new CosmosDbTestContainerResult(
                _cosmosDbContainer.GetConnectionString(),
                () => new HttpClient(_cosmosDbContainer.HttpMessageHandler, disposeHandler: false));
        return result;
    }
    
    public Task Stop() => _cosmosDbContainer?.StopAsync() ?? Task.CompletedTask;
}

public record CosmosDbTestContainerResult(string ConnectionString, Func<HttpClient> HttpClient);