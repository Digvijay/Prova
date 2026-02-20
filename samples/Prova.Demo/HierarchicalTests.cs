using Prova;
using System.Threading.Tasks;


namespace Prova.Demo
{
    // Inherits Assembly defaults
    public class SimpleConfigTests
    {
        [Fact]
        public async Task UsesGlobalDefaults()
        {
            await Task.Delay(100);
            // Will fail once then pass due to Global Retry(2)
        }
    }

    [Timeout(100)] // Class Override: strict timeout
    public class PerformanceCriticalTests
    {
        [Fact]
        public async Task MustRunFast()
        {
            await Task.Delay(10); // Pass
        }

        [Fact]
        [Timeout(1000)] // Method Override: allow slowness here
        public async Task SlowOperation()
        {
            await Task.Delay(500); // Pass (would fail under Class timeout)
        }
    }

    [Retry(0)] // Disable retries for this class
    public class FlakyTestsDisabled
    {
        [Fact]
        public void NoRetryOnFailure()
        {
            // If this fails, it fails immediately
        }
    }
}
