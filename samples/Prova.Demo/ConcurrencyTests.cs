using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    /// <summary>
    /// Tests for validating concurrency controls.
    /// </summary>
    [Parallel(max: 1)]
    public class ConcurrencyTests
    {
        /// <summary>
        /// A sequential test.
        /// </summary>
        [Fact]
        public async Task SequentialTest1()
        {
            await Task.Delay(500);
        }

        /// <summary>
        /// Another sequential test.
        /// </summary>
        [Fact]
        public async Task SequentialTest2()
        {
            await Task.Delay(500);
        }
    }
}
