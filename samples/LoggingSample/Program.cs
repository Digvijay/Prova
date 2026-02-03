using Prova;
using System;
using System.Threading.Tasks;

// Run tests
await Prova.TestRunnerExecutor.RunAllAsync(args);

public class LogTests
{
    [Fact]
    public async Task ShouldLogExpliticly()
    {
        var logger = TestContext.Current.Logger;
        logger.Log("Hello from test!");
        logger.LogWarning("This is a warning.");
        logger.LogError("This is an error.");
        await Task.CompletedTask;
    }
}
