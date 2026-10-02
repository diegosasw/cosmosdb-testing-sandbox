using Sample.ConsoleApp;
using Test.Common.Fixtures;

namespace IntegrationTests.Contracts;

public abstract class IntegrationTestBase
{
    [ClassDataSource<CosmosEmulatorFixture>(Shared = SharedType.PerTestSession)]
    public required CosmosEmulatorFixture Emulator { get; init; }

    protected async Task RunWithEmulatorAsync(
        Func<CosmosEmulatorConnection, Task> test, DatabaseScope? scope = null)
    {
        await using var database = await Emulator.CreateDatabaseAsync(
            GetType(), TestContext.Current!.Isolation.UniqueId, scope);
        await test(new CosmosEmulatorConnection(
            database.Connection.Endpoint, database.Connection.AccountKey, database.DatabaseName));
    }
}
