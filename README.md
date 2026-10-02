# Cosmos DB emulator testing sample

This .NET 10 solution runs a small console program against the Linux Azure Cosmos DB vNext emulator and tests it with TUnit. Docker with Linux containers is required. The emulator supports the NoSQL API in gateway mode; the .NET SDK requires its HTTPS mode.

## Run the console sample

```powershell
docker compose up -d
```

Wait until `http://localhost:8080/ready` responds successfully, then run:

```powershell
dotnet run --project src/Sample.Console -- "hello cosmos"
```

The console program uses `https://localhost:8081/`, the emulator's public key, and database `sample-console` by default. It creates the database and `Documents` container and inserts one document. There is no app settings file or emulator switch. If your emulator runs at another address, set `COSMOS_EMULATOR_ENDPOINT`, `COSMOS_EMULATOR_KEY`, or `COSMOS_DATABASE` as environment variables. Stop the local emulator with `docker compose down`.

## Run the integration tests

```powershell
dotnet build cosmosdb-testing-sandbox.slnx
dotnet run --project test/IntegrationTests -- --list-tests
dotnet run --project test/IntegrationTests
```

Tests launch their own emulator through Testcontainers, obtain its mapped endpoint and key, and pass those values directly to the same console sample logic. The session fixture provisions each test database before running the sample and deletes per-test databases afterward. It disposes the emulator containers at the end of the test session. The local Compose emulator is not needed for tests.

The test-only `test/IntegrationTests/appsettings.IntegrationTests.json` selects resource sharing:

| Setting | Values | Default |
| --- | --- | --- |
| `IntegrationTests:EmulatorScope` | `PerRun`, `PerClass` | `PerRun` |
| `IntegrationTests:DatabaseScope` | `PerTest`, `PerClass` | `PerTest` |

For a one-time override, use standard configuration environment variables. For example, in PowerShell:

```powershell
$env:IntegrationTests__EmulatorScope = 'PerClass'
$env:IntegrationTests__DatabaseScope = 'PerTest'
dotnet run --project test/IntegrationTests
Remove-Item Env:IntegrationTests__EmulatorScope, Env:IntegrationTests__DatabaseScope
```

`PerRun` shares one emulator across test classes; `PerClass` starts one per class. `PerTest` creates a database for each case, including every `[Arguments]` row; `PerClass` shares a database within a class. Individual examples can override the database scope. TUnit runs cases in parallel, so shared database tests should not assume exclusive contents.

See the [Linux emulator guide](https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux) and [TUnit fixture guide](https://tunit.dev/docs/writing-tests/class-data-source/) for platform details.
