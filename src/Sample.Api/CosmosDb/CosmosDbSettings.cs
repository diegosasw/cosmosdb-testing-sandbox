namespace Sample.Api.CosmosDb;

public record CosmosDbSettings
{
    public string AccountEndpoint { get; init; } = string.Empty;
    public string PrimaryKey { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;

    public static IEnumerable<string> ContainerNames =>
    [
        "01",
        "02",
        "03",
        "04",
        "05",
        "06",
        "07",
        "08",
        "09",
        "10",
        "11",
        "12"
    ];
}