using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Sample.ConsoleApp;
using Test.Common.TestContainers;
using TUnit.Core.Interfaces;

namespace Test.Common.Fixtures;

public enum ResourceScope { PerRun, PerClass }
public enum DatabaseScope { PerTest, PerClass }

public sealed class IntegrationTestSettings
{
    public ResourceScope EmulatorScope { get; set; } = ResourceScope.PerRun;
    public DatabaseScope DatabaseScope { get; set; } = DatabaseScope.PerTest;
}

public sealed class CosmosEmulatorFixture : IAsyncInitializer, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, Lazy<Task<EmulatorInstance>>> _emulators = new();
    private readonly string _runId = Guid.NewGuid().ToString("N")[..8];

    public IntegrationTestSettings Settings { get; private set; } = null!;

    public Task InitializeAsync()
    {
        Settings = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.IntegrationTests.json", optional: false)
            .AddEnvironmentVariables()
            .Build()
            .GetSection("IntegrationTests")
            .Get<IntegrationTestSettings>() ?? throw new InvalidOperationException("IntegrationTests settings are missing.");
        return Task.CompletedTask;
    }

    public async Task<CosmosDatabaseLease> CreateDatabaseAsync(
        Type testClass, long testId, DatabaseScope? databaseScope = null)
    {
        var className = testClass.FullName ?? testClass.Name;
        var emulatorKey = Settings.EmulatorScope == ResourceScope.PerRun ? "run" : className;
        var emulator = await _emulators.GetOrAdd(emulatorKey,
            _ => new Lazy<Task<EmulatorInstance>>(StartEmulatorAsync)).Value;

        var scope = databaseScope ?? Settings.DatabaseScope;
        var classPart = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(className)))[..12].ToLowerInvariant();
        var databaseName = scope == DatabaseScope.PerTest
            ? $"test-{_runId}-{classPart}-{testId}"
            : $"test-{_runId}-{classPart}";

        await emulator.EnsureDatabaseAsync(databaseName);
        return new CosmosDatabaseLease(emulator, databaseName, scope == DatabaseScope.PerTest);
    }

    private static async Task<EmulatorInstance> StartEmulatorAsync()
    {
        var container = new CosmosDbTestContainer();
        try
        {
            var connection = await container.StartAsync();
            return new EmulatorInstance(container, connection);
        }
        catch
        {
            await container.DisposeAsync();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var entry in _emulators.Values)
        {
            if (!entry.IsValueCreated) continue;
            if (entry.Value.IsCompletedSuccessfully)
                await (await entry.Value).DisposeAsync();
        }
    }
}

public sealed class EmulatorInstance : IAsyncDisposable
{
    private readonly CosmosDbTestContainer _container;
    private readonly CosmosClient _client;
    private readonly ConcurrentDictionary<string, Lazy<Task>> _databases = new();

    public CosmosDbTestContainerResult Connection { get; }

    public EmulatorInstance(CosmosDbTestContainer container, CosmosDbTestContainerResult connection)
    {
        _container = container;
        Connection = connection;
        _client = CosmosEmulatorConnection.CreateClient(connection.Endpoint, connection.AccountKey);
    }

    public Task EnsureDatabaseAsync(string databaseName) =>
        _databases.GetOrAdd(databaseName, name =>
            new Lazy<Task>(() => ProvisionAsync(name))).Value;

    private async Task ProvisionAsync(string name)
    {
        var database = await _client.CreateDatabaseIfNotExistsAsync(name);
        await database.Database.CreateContainerIfNotExistsAsync(CosmosSample.ContainerName, "/id");
    }

    public async Task DeleteDatabaseAsync(string databaseName)
    {
        await _client.GetDatabase(databaseName).DeleteAsync();
        _databases.TryRemove(databaseName, out _);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            foreach (var name in _databases.Keys)
            {
                await DeleteDatabaseAsync(name);
            }
        }
        finally
        {
            _client.Dispose();
            await _container.DisposeAsync();
        }
    }
}

public sealed class CosmosDatabaseLease(EmulatorInstance emulator, string databaseName, bool deleteOnDispose)
    : IAsyncDisposable
{
    public string DatabaseName => databaseName;
    public CosmosDbTestContainerResult Connection => emulator.Connection;

    public async ValueTask DisposeAsync()
    {
        if (deleteOnDispose) await emulator.DeleteDatabaseAsync(databaseName);
    }
}
