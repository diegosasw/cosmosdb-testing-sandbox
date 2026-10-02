using Microsoft.Azure.Cosmos;
using Sample.ConsoleApp.CosmosDb;

namespace Sample.ConsoleApp;

public static class CosmosSample
{
    public const string ContainerName = "Documents";

    public static async Task<InsertResult> InsertAsync(
        CosmosEmulatorConnection connection, string text, CancellationToken cancellationToken = default)
    {
        using var client = connection.CreateClient();
        var database = await client.CreateDatabaseIfNotExistsAsync(
            connection.DatabaseName, cancellationToken: cancellationToken);
        var container = await database.Database.CreateContainerIfNotExistsAsync(
            ContainerName, "/id", cancellationToken: cancellationToken);

        var document = new CosmosDbDocument(Guid.NewGuid().ToString("N"), text, DateTime.UtcNow);
        await container.Container.CreateItemAsync(document, new PartitionKey(document.id),
            cancellationToken: cancellationToken);

        return new InsertResult(connection.DatabaseName, ContainerName,
            document.id, document.text);
    }
}

public sealed record InsertResult(string DatabaseName, string ContainerName, string DocumentId, string Text);
