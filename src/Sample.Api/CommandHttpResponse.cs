namespace Sample.Api;

public record CommandHttpResponse(string DatabaseName, string ContainerName, CommandHttpRequest Payload);