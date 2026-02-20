using Prova;
using System;
using System.Threading.Tasks;

// Top-Level Statements: acts as the "Script" entry point
Console.WriteLine("🚀 Running Scripted Tests...");

// Optional: Configure global settings via code if needed
// Prova.Configuration.Config.DefaultTimeoutMs = 500;

// Execute Tests
await Prova.TestRunnerExecutor.RunAllAsync(args);

Console.WriteLine("✅ Script Completed.");

// Test Definitions in the same file
namespace Scripting
{
    public class SimpleTests
    {
        [Fact]
        public void MathCheck()
        {
            Assert.Equal(4, 2 + 2);
        }

        [Fact]
        public async Task AsyncCheck()
        {
            await Task.Delay(10);
            Assert.True(true);
        }
    }

    public class FixtureTests
    {
        [Before]
        public void Setup()
        {
            Console.WriteLine("  [Script] Setup");
        }

        [Fact]
        public void TestWithContext()
        {
            Console.WriteLine($"  [Script] Running {TestContext.Current?.DisplayName}");
        }
    }
}
