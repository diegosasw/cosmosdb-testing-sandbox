using LightBDD.Core.Extensibility.Execution;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sample.Api;
using Test.Common;
using Test.Common.Fixtures;
using Xunit.Abstractions;

// ReSharper disable MemberCanBePrivate.Global

namespace SpecificationTests.Contracts;

public abstract class SpecificationTestBase 
    : TestServerBase<Program>
{
    protected SpecificationTestBase(string testId, TestContainerFixture fixture, ITestOutputHelper output, bool clearContainers) : base(testId, fixture, output, clearContainers)
    {
    }
}


// public abstract class SpecificationTestBase
//     : FeatureFixture, IScenarioSetUp, IScenarioTearDown, IDisposable
// {
//     public HttpClient HttpClient
//         => _webApplicationFactory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
//     
//     public readonly IServiceProvider Services;
//     
//     public string TestId { get; }
//     
//     private readonly WebApplicationFactory<Program>? _webApplicationFactory;
//
//     protected SpecificationTestBase()
//     {
//         TestId = Guid.NewGuid().ToString();
//         _webApplicationFactory = 
//             TestServerFactory.CreateWebApplicationFactory<Program>(TestId, "service-stored-recipient-spectests");
//         
//         Services = _webApplicationFactory.Services.CreateScope().ServiceProvider;
//     }
//
//     public Task OnScenarioSetUp()
//     {
//         return Task.CompletedTask;
//     }
//
//     public async Task OnScenarioTearDown()
//     {
//         await DeleteTestDatabaseContainers();
//     }
//
//     public void Dispose()
//     {
//     }
//     
//     private async Task DeleteTestDatabaseContainers()
//     {
//         var context = Services.GetRequiredService<CosmosDbContext>();
//         _ = await context.RemoveContainers();
//     }
// }