// ReSharper disable InconsistentNaming
namespace Sample.Api.CosmosDb;

public record CosmosDbDocument(
    string id, 
    string text, 
    DateTime treatedAt);