namespace Sample.Api.CosmosDb;

public record CosmosOperationResult
{
    public bool IsSuccessful { get; }
    public string ErrorMessage { get; }

    private CosmosOperationResult(bool isSuccessful, string errorMessage)
    {
        IsSuccessful = isSuccessful;
        ErrorMessage = errorMessage;
    }
    
    public static CosmosOperationResult Success() => new (true, string.Empty);
    public static CosmosOperationResult Error(string error) => new (false, error);
}