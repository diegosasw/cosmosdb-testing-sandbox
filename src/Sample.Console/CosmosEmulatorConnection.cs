using Microsoft.Azure.Cosmos;

namespace Sample.ConsoleApp;

public sealed record CosmosEmulatorConnection(string Endpoint, string AccountKey, string DatabaseName)
{
    // The emulator key is public and fixed. It is not an Azure account credential.
    public const string DefaultAccountKey =
        "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    public static CosmosEmulatorConnection FromEnvironment() => new(
        Environment.GetEnvironmentVariable("COSMOS_EMULATOR_ENDPOINT") ?? "https://localhost:8081/",
        Environment.GetEnvironmentVariable("COSMOS_EMULATOR_KEY") ?? DefaultAccountKey,
        Environment.GetEnvironmentVariable("COSMOS_DATABASE") ?? "sample-console");

    public CosmosClient CreateClient()
    {
        if (string.IsNullOrWhiteSpace(DatabaseName))
            throw new ArgumentException("Emulator database name is required.");
        return CreateClient(Endpoint, AccountKey);
    }

    public static CosmosClient CreateClient(string endpoint, string accountKey)
    {
        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(accountKey))
            throw new ArgumentException("Emulator endpoint and key are required.");

        return new CosmosClient(endpoint, accountKey, new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            LimitToEndpoint = true,
            HttpClientFactory = () => new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
        });
    }
}
