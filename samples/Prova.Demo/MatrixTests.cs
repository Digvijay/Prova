using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class MatrixTests
    {
        // Runs 2 x 2 = 4 times
        [Fact]
        public void Combinatorial_Primes([Matrix(2, 3)] int x, [Matrix(5, 7)] int y)
        {
            // Just verifying it runs
            Assert.True(x * y > 0);
        }

        // Runs 2 x 2 = 4 times
        [Fact]
        public async Task Async_Combinations([Matrix("a", "b")] string s, [Matrix(true, false)] bool flag)
        {
            await Task.Delay(10);
            if (flag) Assert.NotNull(s);
        }

        public enum Mode { Fast, Slow }

        // Runs 2 x 3 = 6 times
        [Fact]
        public void Enums_And_Ints([Matrix(Mode.Fast, Mode.Slow)] Mode mode, [Matrix(1, 10, 100)] int count)
        {
            Assert.True(count > 0);
        }
    }
}
