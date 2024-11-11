using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sample.Api.CosmosDb;
using Test.Common.Fixtures;
using Xunit.Abstractions;

namespace Test.Common;

public abstract class TestServerBase<TEntryPoint>
    : GivenWhenThen
    where TEntryPoint : class
{
    private readonly ITestOutputHelper _output;
    private readonly bool _clearContainers;

    protected HttpClient HttpClient
        => _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    
    private readonly WebApplicationFactory<TEntryPoint> _webApplicationFactory;
    private readonly IServiceProvider _services;

    protected TestServerBase(string testId, TestContainerFixture fixture, ITestOutputHelper output, bool clearContainers)
    {
        _output = output;
        _clearContainers = clearContainers;
        
        _webApplicationFactory = 
            TestServerFactory.CreateWebApplicationFactory<TEntryPoint>(
                testId, 
                fixture.CosmosDbTestContainerResult,
                TestServices);
        _services = _webApplicationFactory.Services.CreateScope().ServiceProvider;
        output.WriteLine($"Running test {testId} on CosmosDb server");
    }

    protected sealed override async Task PreConditions()
    {
        await base.PreConditions();

        if (!_clearContainers)
        {
            return;
        }

        var cosmosManager = _services.GetRequiredService<CosmosManager>();
        var cosmosSettings = _services.GetRequiredService<CosmosDbSettings>();
        foreach (var containerName in CosmosDbSettings.ContainerNames)
        {
            _output.WriteLine($"Clearing container {containerName}");
            await cosmosManager.ClearContainerAsync(cosmosSettings.DatabaseName, containerName);
            _output.WriteLine($"Container {containerName} cleared");
        }
    }

    protected virtual void TestServices(IServiceCollection services, string testId)
    {
    }
}