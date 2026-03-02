using Prova;
using System;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class FlowControlSample
    {
        private static int _flakyRuns;

        [Fact]
        [Retry(3)]
        public void FlakyTest()
        {
            _flakyRuns++;
            if (_flakyRuns < 3)
            {
                throw new InvalidOperationException("Simulated flaky failure");
            }
            // Passes on 3rd attempt
        }

        [Fact]
        [Timeout(200)]
        public async Task TimeoutTest()
        {
            await Task.Delay(100); // Should pass
        }

        [Fact]
        [Timeout(50)]
        [Trait("Category", "Fails")] // Expected to fail
        public async Task TimeoutFailureTest()
        {
            await Task.Delay(100);
        }

        [Fact]
        [Repeat(5)]
        public void RepeatTest()
        {
            // Runs 5 times
        }

        [Fact]
        [Culture("fr-FR")]
        public void CultureTest()
        {
            var date = new DateTime(2023, 10, 25);
            // In French culture, it should be 25/10/2023
            if (date.ToShortDateString() != "25/10/2023")
            {
                throw new InvalidOperationException($"Culture check failed: {date.ToShortDateString()}");
            }
        }
    }
}
