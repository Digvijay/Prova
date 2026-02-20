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
        /// A synchronous test that exceeds its timeout.
        /// </summary>
        [Fact(Skip = "Intentional failure for verification")]
        [Timeout(100)]
        public void Sync_Test_Should_Fail()
        {
            System.Threading.Thread.Sleep(500);
        }
    }
}
