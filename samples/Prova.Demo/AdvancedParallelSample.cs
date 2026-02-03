using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class AdvancedParallelSample
    {
        [Fact]
        [ParallelLimiter("Database", 2)]
        public async Task DB_Test1()
        {
            await Task.Delay(500);
            Assert.True(true);
        }

        [Fact]
        [ParallelLimiter("Database", 2)]
        public async Task DB_Test2()
        {
            await Task.Delay(500);
            Assert.True(true);
        }

        [Fact]
        [ParallelLimiter("Database", 2)]
        public async Task DB_Test3()
        {
            await Task.Delay(500);
            Assert.True(true);
        }

        [Fact]
        [ParallelGroup("Reports")]
        public void Report_Test1() { }

        [Fact]
        [ParallelGroup("Reports")]
        public void Report_Test2() { }
    }
}
