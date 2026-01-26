using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Core.Tests
{
    public class SampleTests
    {
        [Fact]
        public static void PassingTest()
        {
            Assert.Equal(1, 1);
        }

        [Fact(Skip = "Intentional failure for framework verification")]
        public static void FailingTest()
        {
            Assert.Equal(1, 2);
        }

        [Fact]
        public static async Task AsyncPassingTest()
        {
            await Task.Delay(10);
            Assert.True(true);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public static void TheoryTest(int a, int b)
        {
            Assert.Equal(a, b);
        }

        [Theory(Skip = "Intentional failure for framework verification")]
        [InlineData(1, 2)]
        public static void FailedTheoryTest(int a, int b)
        {
            Assert.Equal(a, b);
        }
        [Fact]
        public static void ThrowsTest()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => throw new InvalidOperationException("Boom"));
            Assert.Equal("Boom", ex.Message);
        }

        [Fact]
        public static async Task ThrowsAsyncTest()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            {
                await Task.Yield();
                throw new InvalidOperationException("Boom Async"); 
            });
        }
    }
}
