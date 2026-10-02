using DotNet.Testcontainers.Builders;
using Testcontainers.CosmosDb;

namespace Test.Common.TestContainers;

public sealed class CosmosDbTestContainer : IAsyncDisposable
{
    private const string Image = "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-latest";
    private const ushort CosmosPort = 8081;
    private const ushort HealthPort = 8080;

    private readonly CosmosDbContainer _container = new CosmosDbBuilder(Image)
        .WithCommand("--protocol", "https")
        .WithPortBinding(HealthPort, true)
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilHttpRequestIsSucceeded(request => request
                .ForPort(HealthPort)
                .ForPath("/ready")))
        .Build();

    public async Task<CosmosDbTestContainerResult> StartAsync()
    {
        await _container.StartAsync();
        var endpoint = new UriBuilder(Uri.UriSchemeHttps, _container.Hostname,
            _container.GetMappedPublicPort(CosmosPort)).Uri.ToString();
        return new CosmosDbTestContainerResult(endpoint, CosmosDbBuilder.DefaultAccountKey);
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}

public sealed record CosmosDbTestContainerResult(string Endpoint, string AccountKey);
