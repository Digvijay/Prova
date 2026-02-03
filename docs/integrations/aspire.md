# Aspire Integration

Prova supports unit and integration testing of **.NET Aspire** applications using the `Aspire.Hosting.Testing` library.

## Prerequisites

- .NET 10 SDK (or later)
- .NET Aspire Workload (`dotnet workload install aspire`)
- Docker (for resources like Redis, Postgres, etc.)

## Usage

To test an Aspire application, create a Prova test project and reference your **AppHost** project.

### 1. Project Configuration

Add the following properties to your test project's `.csproj`:

```xml
<PropertyGroup>
  <IsTestProject>true</IsTestProject>
  <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
  <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
  <IsTestingPlatformApplication>true</IsTestingPlatformApplication>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Aspire.Hosting.Testing" Version="9.0.0" />
</ItemGroup>
```

### 2. Entry Point

Prova requires an entry point in your test project (`Program.cs`):

```csharp
await Prova.TestRunnerExecutor.RunAllAsync(args);
```

### 3. Integration Test

Use `DistributedApplicationTestingBuilder` to spin up the AppHost. We recommend implementing `IAsyncLifetime` for clean setup and teardown.

```csharp
using Aspire.Hosting.Testing;
using Prova;

public class MyAspireTests : IAsyncLifetime
{
    private DistributedApplication? _app;

    public async Task InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.My_AppHost>();
        _app = await builder.BuildAsync();
        await _app.StartAsync();
    }

    [Fact]
    public async Task Service_Is_Reachable()
    {
        var client = _app.CreateHttpClient("api");
        var response = await client.GetAsync("/health");
        Assert.True(response.IsSuccessStatusCode);
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
        }
    }
}
```

## Troubleshooting

### Missing DCP Binaries
If you see errors like `Property CliPath is required`, ensure the Aspire workload is installed correcty. If running in a CI environment without the workload, you may need to manually provide the paths to the DCP binaries via environment variables:
- `ASPIRE_DCP_CLI_PATH`
- `ASPIRE_DASHBOARD_PATH`
