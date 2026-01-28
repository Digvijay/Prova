using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Demo
{
    /// <summary>
    /// Demonstrates Prova's [Timeout] attribute capability.
    /// </summary>
    public class TimeoutSample
    {
        /// <summary>
        /// A test that completes well within the timeout limit.
        /// </summary>
        [Fact]
        [Timeout(1000)] // 1s timeout
        public async Task Fast_Test_Should_Pass()
        {
            await Task.Delay(100);
        }

        /// <summary>
        /// A test designed to exceed its timeout limit and fail.
        /// </summary>
        [Fact(Skip = "Intentional failure for verification")]
        [Timeout(50)] // 50ms timeout
        public async Task Slow_Test_Should_Fail()
        {
            // This runs for 500ms, which is > 50ms
            await Task.Delay(500); 
        }
    }
}
