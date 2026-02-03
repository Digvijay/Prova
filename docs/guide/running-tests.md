# Running Tests

Since Prova tests are executable binaries, you have two primary ways to run them.

## 1. `dotnet test`
This is the standard way to run tests in the .NET ecosystem and is compatible with CI/CD pipelines.

```bash
dotnet test
```

### Filtering
You can filter tests using the standard `--filter` syntax.

```bash
dotnet test --filter "FullyQualifiedName~SmokeTests"
```

## 2. Direct Execution
For maximum speed and debugging, you can run the compiled executable directly. This bypasses the MSBuild overhead.

```bash
# Debug Build
./bin/Debug/net8.0/Prova.Tests

# Release/AOT Build
./bin/Release/net8.0/publish/Prova.Tests
```

## Parallelism
By default, Prova runs all tests in parallel. You can control this via command line arguments or global configuration.

> [!NOTE]
> Detailed configuration options are available in the **[Configuration API](../api/config.md)** section.
