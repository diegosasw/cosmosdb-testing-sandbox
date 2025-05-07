# cosmosdb-testing-sandbox
Sandbox for testing with CosmosDb Emulator in its Docker Image version

## Pre-Requirements
- .NET 8 SDK
- Docker

## How to run  
Compile with
```
dotnet build
```

Run tests with
```
dotnet test
```

The tests will run a TestContainer for CosmosDb emulator once before all tests, the tests will run sequentially and then the docker container will be destroyed
Play with the CosmosDb configuration, the `AZURE_COSMOS_EMULATOR_PARTITION_COUNT`, etc. and observe how unreliable the CosmosDb emulator is.
Add heavier load to the tests and observe `Too Many Request` and other server errors.

