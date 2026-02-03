using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    // This class runs its tests with a specific limit of 2 concurrent tests.
    // Other classes in the suite can still run in parallel with these.
    [Parallel(2)]
    public class LimitedParallelismTests
    {
        [Fact]
        public async Task Test1()
        {
            await Task.Delay(100);
        }

        [Fact]
        public async Task Test2()
        {
            await Task.Delay(100);
        }

        [Fact]
        public async Task Test3()
        {
            await Task.Delay(100);
        }
    }

    // This class runs its tests serially (one at a time).
    [Sequential]
    public class SerialTests
    {
        [Fact]
        public async Task Serial1()
        {
            await Task.Delay(50);
        }

        [Fact]
        public async Task Serial2()
        {
            await Task.Delay(50);
        }
    }

    // This test will run in complete isolation.
    // No other tests from any class will be running while this executes.
    public class IsolatedTests
    {
        [Fact, DoNotParallelize]
        public async Task TheLoner()
        {
            // I am all alone!
            await Task.Delay(200);
        }
    }
}
