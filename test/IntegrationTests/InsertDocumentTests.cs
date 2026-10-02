using IntegrationTests.Contracts;
using Sample.ConsoleApp;
using Test.Common;
using Test.Common.Fixtures;

namespace IntegrationTests;

public sealed class InsertDocumentTests : IntegrationTestBase
{
    [Test]
    [Arguments("one")]
    [Arguments("two")]
    [Arguments("three")]
    public async Task Each_case_inserts_into_its_own_database(string text)
    {
        await RunWithEmulatorAsync(async connection =>
        {
            var result = await CosmosSample.InsertAsync(connection, text);
            await Assert.That(result.DatabaseName).IsEqualTo(connection.DatabaseName);
            await Assert.That(result.ContainerName).IsEqualTo(CosmosSample.ContainerName);
            await Assert.That(result.DocumentId).IsNotEmpty();
            await Assert.That(result.Text).IsEqualTo(text);
        }, DatabaseScope.PerTest);
    }

    [Test]
    [Arguments("shared-one")]
    [Arguments("shared-two")]
    public async Task Class_scope_inserts_into_a_shared_database(string text)
    {
        await RunWithEmulatorAsync(async connection =>
        {
            var result = await CosmosSample.InsertAsync(connection, text);
            await Assert.That(result.DatabaseName).IsEqualTo(connection.DatabaseName);
            await Assert.That(result.Text).IsEqualTo(text);
        }, DatabaseScope.PerClass);
    }
}

public sealed class DatabaseIsolationTests : IntegrationTestBase
{
    [Test]
    public async Task Per_test_names_are_distinct_and_per_class_names_match()
    {
        var testId = TestContext.Current!.Isolation.UniqueId;
        await using var first = await Emulator.CreateDatabaseAsync(GetType(), testId, DatabaseScope.PerTest);
        await using var second = await Emulator.CreateDatabaseAsync(GetType(), testId + 1_000_000, DatabaseScope.PerTest);
        await using var classFirst = await Emulator.CreateDatabaseAsync(GetType(), testId, DatabaseScope.PerClass);
        await using var classSecond = await Emulator.CreateDatabaseAsync(GetType(), testId + 1_000_000, DatabaseScope.PerClass);
        await Assert.That(first.DatabaseName).IsNotEqualTo(second.DatabaseName);
        await Assert.That(classFirst.DatabaseName).IsEqualTo(classSecond.DatabaseName);
    }
}

public sealed class GivenWhenThenExample : IntegrationTestBase
{
    [Test]
    public Task Document_is_inserted() => new InsertScenario(this).RunAsync();

    private sealed class InsertScenario(GivenWhenThenExample owner) : GivenWhenThen
    {
        private string _text = null!;
        private CosmosDatabaseLease _database = null!;
        private InsertResult _result = null!;

        public Task RunAsync() => RunScenarioAsync();

        protected override Task Given()
        {
            _text = "scenario";
            return Task.CompletedTask;
        }

        protected override async Task When()
        {
            _database = await owner.Emulator.CreateDatabaseAsync(
                owner.GetType(), TestContext.Current!.Isolation.UniqueId);
            var connection = new CosmosEmulatorConnection(
                _database.Connection.Endpoint, _database.Connection.AccountKey, _database.DatabaseName);
            _result = await CosmosSample.InsertAsync(connection, _text);
        }

        protected override async Task Then()
        {
            await Assert.That(_result.DatabaseName).IsEqualTo(_database.DatabaseName);
            await Assert.That(_result.Text).IsEqualTo(_text);
        }

        protected override async Task Cleanup()
        {
            if (_database is not null) await _database.DisposeAsync();
        }
    }
}
