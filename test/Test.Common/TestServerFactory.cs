using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Common;

public static class TestServerFactory
{
    public static WebApplicationFactory<TEntryPoint> CreateWebApplicationFactory<TEntryPoint>(
        string testId, 
        string testDatabaseName,
        Action<IServiceCollection, string>? testServices = default)
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
                            testServices?.Invoke(services, testId);
                        }));

        return webApplicationFactory;
    }
}