using System;
using System.Threading.Tasks;
using Prova;

// Run tests
return await Prova.TestRunnerExecutor.RunAllAsync(args);

namespace LoggingSample
{
    public class LogTests
    {
        [Fact]
        public async Task ShouldLogExplicitly()
        {
            var logger = TestContext.Current.Logger;
            logger.Log("Hello from test!");
            logger.LogWarning("This is a warning.");
            logger.LogError("This is an error.");
            await Task.CompletedTask;
        }
    }
}