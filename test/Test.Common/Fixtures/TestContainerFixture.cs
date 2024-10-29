using Testcontainers.CosmosDb;
using Xunit;

// ReSharper disable ClassNeverInstantiated.Global

namespace Test.Common.Fixtures;

public class TestContainerFixture
    : IAsyncLifetime
{
    public string CosmosDbConnectionString { get; private set; } = null!;
    public HttpClient CosmosDbHttpClient => new(_cosmosDbHttpMessageHandler, disposeHandler: false);
    
    private CosmosDbContainer _cosmosDbContainer = null!;
    private HttpMessageHandler _cosmosDbHttpMessageHandler = null!;
    
    public async Task InitializeAsync()
    {
        _cosmosDbContainer = 
            new CosmosDbBuilder()
                .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE","127.0.0.1")
                .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "false")
                .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "12")
                .Build();
        await _cosmosDbContainer.StartAsync();
        CosmosDbConnectionString = _cosmosDbContainer.GetConnectionString();
        _cosmosDbHttpMessageHandler = _cosmosDbContainer.HttpMessageHandler;
    }

    public Task DisposeAsync() => _cosmosDbContainer.StopAsync();
}