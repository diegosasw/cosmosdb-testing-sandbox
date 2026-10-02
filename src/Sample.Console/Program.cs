using Sample.ConsoleApp;

var text = args.Length == 0 ? "hello from the emulator" : string.Join(' ', args);
var connection = CosmosEmulatorConnection.FromEnvironment();
var result = await CosmosSample.InsertAsync(connection, text);

Console.WriteLine($"Inserted {result.DocumentId} into {result.DatabaseName}/{result.ContainerName}: {result.Text}");
