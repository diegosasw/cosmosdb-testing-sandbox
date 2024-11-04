using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Api.CosmosDb;
using Test.Common.Extensions;
using Test.Common.TestContainers;

namespace Test.Common;

public static class TestServerFactory
{
    public static WebApplicationFactory<TEntryPoint> CreateWebApplicationFactory<TEntryPoint>(
        string testId,
        CosmosDbTestContainerResult cosmosDbTestContainerResult,
        Action<IServiceCollection, string>? additionalTestServices = default)
        where TEntryPoint : class
    {
        var configurationBuilder =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new List<KeyValuePair<string, string?>>
                    {
                        new("TestId", testId)
                    })
                .Build();

        var webApplicationFactory =
            new WebApplicationFactory<TEntryPoint>()
                .WithWebHostBuilder(webHostBuilder =>
                    webHostBuilder
                        .UseEnvironment("Development")
                        .UseConfiguration(configurationBuilder)
                        .ConfigureServices(services =>
                        {
                            // Overriding infrastructure services
                            var cosmosDbTestSettings =
                                new CosmosDbSettings
                                {
                                    AccountEndpoint = string.Empty, // unused for test
                                    PrimaryKey = string.Empty, // unused for test,
                                    DatabaseName = "test-database"
                                };
                    
                            services.ReplaceService(cosmosDbTestSettings);
                    
                            var cosmosClientTestOptions =
                                new CosmosClientOptions
                                {
                                    ConnectionMode = ConnectionMode.Gateway,
                                    HttpClientFactory = cosmosDbTestContainerResult.HttpClient
                                };
                    
                            var cosmosDbTestClient = new CosmosClient(cosmosDbTestContainerResult.ConnectionString, cosmosClientTestOptions);
                    
                            services.ReplaceService(cosmosDbTestClient);
                            
                            additionalTestServices?.Invoke(services, testId);
                        }));

        return webApplicationFactory;
    }
}