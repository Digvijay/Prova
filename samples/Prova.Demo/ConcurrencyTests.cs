using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    [Parallel(max: 1)]
    public class ConcurrencyTests
    {
        [Fact]
        public async Task SequentialTest1()
        {
            await Task.Delay(500);
        }

        [Fact]
        public async Task SequentialTest2()
        {
            await Task.Delay(500);
        }
    }
}
