# Advanced Logging

Prova provides a built-in logging abstraction that allows you to write structured logs from your tests. These logs are exposed via `TestContext` and can be captured by the test runner or custom reporters.

## Usage

Access the logger via `TestContext.Current.Logger`:

```csharp
using Prova;
using System.Threading.Tasks;

public class MyTests
{
    [Fact]
    public async Task ProcessData()
    {
        var logger = TestContext.Current.Logger;
        
        logger.Log("Starting processing...");
        
        try
        {
            await Service.DoWork();
        }
        catch (Exception ex)
        {
            logger.LogError($"Processing failed: {ex.Message}");
            throw;
        }
        
        logger.Log("Processing complete.");
    }
}
```

## Logger Interface

The `ITestLogger` interface provides three levels of logging:

- `Log(string message)`: General information.
- `LogWarning(string message)`: Warnings that don't fail the test but should be noted.
- `LogError(string message)`: Errors (often accompanied by a failure).

## Default Behavior

By default, Prova uses a `ConsoleLogger` which writes appropriately colored output to the console.

### CI/CD Integration

The `ConsoleLogger` automatically detects when it is running in **GitHub Actions** (`GITHUB_ACTIONS=true`) or **Azure DevOps** (`TF_BUILD=True`). When detected, it emits platform-specific service messages for warnings and errors (e.g., `::warning::`, `::error::`, `##vso[task.logissue]`) so that they are properly annotated in the CI build summary.
