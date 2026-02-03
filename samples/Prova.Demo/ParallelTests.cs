using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    [Sequential]
    public class SequentialTests
    {
        [Fact]
        public async Task Test1()
        {
            await Task.Delay(500);
        }

        [Fact]
        public async Task Test2()
        {
            await Task.Delay(500);
        }
    }

    [Parallel(2)]
    public class ParallelLimitedTests
    {
        [Fact]
        public async Task TestA() { await Task.Delay(500); }

        [Fact]
        public async Task TestB() { await Task.Delay(500); }

        [Fact]
        public async Task TestC() { await Task.Delay(500); }
    }

    [Parallel]
    public class ParallelUnlimitedTests
    {
        [Fact]
        public async Task Fast1() { await Task.Delay(100); }
        
        [Fact]
        public async Task Fast2() { await Task.Delay(100); }
    }
}
