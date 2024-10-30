using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Api.CosmosDb;
using Test.Common.Extensions;
using Test.Common.Fixtures;
using Xunit.Abstractions;

namespace Test.Common;

public abstract class TestServerBase<TEntryPoint>
    : GivenWhenThen
    where TEntryPoint : class
{
    private readonly string _testId;
    private readonly TestContainerFixture _fixture;
    private readonly ITestOutputHelper _output;

    protected HttpClient HttpClient 
    => _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    
    private readonly WebApplicationFactory<TEntryPoint> _webApplicationFactory;
    private readonly IServiceProvider _services;

    protected TestServerBase(string testId, TestContainerFixture fixture, ITestOutputHelper output)
    {
        _testId = testId;
        _fixture = fixture;
        _output = output;
        
        _webApplicationFactory = 
            TestServerFactory.CreateWebApplicationFactory<TEntryPoint>(
                testId, 
                (sc, id) =>
                {
                    // Overriding infrastructure services
                    var cosmosDbTestSettings =
                        new CosmosDbSettings
                        {
                            AccountEndpoint = string.Empty, // unused for test
                            PrimaryKey = string.Empty, // unused for test,
                            DatabaseName = "test-database"
                        };
                    
                    sc.ReplaceService(cosmosDbTestSettings);
                    
                    var cosmosClientTestOptions =
                        new CosmosClientOptions
                        {
                            ConnectionMode = ConnectionMode.Gateway,
                            HttpClientFactory = () => fixture.CosmosDbHttpClient
                        };
                    
                    var cosmosDbTestClient = new CosmosClient(fixture.CosmosDbConnectionString, cosmosClientTestOptions);
                    
                    sc.ReplaceService(cosmosDbTestClient);
                    
                    // Additional Test Services    
                    TestServices(sc, id);
                });
        _services = _webApplicationFactory.Services.CreateScope().ServiceProvider;
        output.WriteLine($"Running test {testId} on CosmosDb server");
    }
    
    protected virtual void TestServices(IServiceCollection services, string testId)
    {
    }
}