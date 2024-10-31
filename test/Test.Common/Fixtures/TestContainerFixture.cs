using Test.Common.TestContainers;
using Testcontainers.CosmosDb;
using Xunit;

// ReSharper disable ClassNeverInstantiated.Global

namespace Test.Common.Fixtures;

public class TestContainerFixture
    : IAsyncLifetime
{
    public CosmosDbTestContainerResult CosmosDbTestContainerResult { get; private set; } = null!;
    
    private CosmosDbTestContainer? _cosmosDbTestContainer;

    public async Task InitializeAsync()
    {
        _cosmosDbTestContainer = new CosmosDbTestContainer();
        CosmosDbTestContainerResult = await _cosmosDbTestContainer.Start();
    }

    public Task DisposeAsync() => _cosmosDbTestContainer?.Stop() ?? Task.CompletedTask;
}