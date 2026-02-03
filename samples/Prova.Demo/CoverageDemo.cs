using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    /// <summary>
    /// Demonstrates the integrated code coverage feature.
    /// Run with: dotnet run -- --coverage
    /// This will generate a coverage.lcov file with execution data.
    /// </summary>
    public class CoverageDemo
    {
        [Fact]
        public async Task Covered_Test_Records_Hit()
        {
            // This test will be tracked by CoverageRegistry
            // The Hit probe is injected at the start of execution
            await Task.Delay(1);
            Assert.True(true);
        }

        [Fact]
        public async Task Another_Covered_Test()
        {
            // Each test gets a unique CoverageId
            var result = 2 + 2;
            Assert.Equal(4, result);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(10, 20, 30)]
        public async Task Theory_Cases_Are_Tracked_Separately(int a, int b, int expected)
        {
            // Each InlineData case gets its own CoverageId
            await Task.CompletedTask;
            Assert.Equal(expected, a + b);
        }
    }
}
