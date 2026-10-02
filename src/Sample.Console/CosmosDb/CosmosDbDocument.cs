// ReSharper disable InconsistentNaming
namespace Sample.ConsoleApp.CosmosDb;

public record CosmosDbDocument(
    string id, 
    string text, 
    DateTime treatedAt);
