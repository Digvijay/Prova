using Prova;

// Set global defaults for the entire assembly
[assembly: Retry(2)]
[assembly: Timeout(5000)]
[assembly: Parallel(4)]

namespace Prova.Demo
{
    public class GlobalPoliciesSample
    {
        private static int _attempt = 0;

        [Fact]
        public void RetriedTest()
        {
            _attempt++;
            if (_attempt < 3)
            {
                throw new Exception("Simulated flaky failure");
            }
            Console.WriteLine($"Passed on attempt {_attempt}");
        }

        [Fact]
        [Timeout(100)] // Override assembly-level 5000ms
        public async Task FastTimeoutTest()
        {
            await Task.Delay(200); // This will fail
        }

        [Fact]
        [Sequential] // Override assembly-level Parallel(4)
        public void SequentialTest()
        {
            // This test will run sequentially
        }
    }
}
