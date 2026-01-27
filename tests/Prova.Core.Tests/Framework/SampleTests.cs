using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Sample tests demonstrating various framework features.
    /// </summary>
    public class SampleTests
    {
        /// <summary>A passing test.</summary>
        [Fact]
        public static void PassingTest()
        {
            Assert.Equal(1, 1);
        }

        /// <summary>A failing test.</summary>
        [Fact(Skip = "Intentional failure for framework verification")]
        public static void FailingTest()
        {
            Assert.Equal(1, 2);
        }

        /// <summary>An async passing test.</summary>
        [Fact]
        public static async Task AsyncPassingTest()
        {
            await Task.Delay(10);
            Assert.True(true);
        }

        /// <summary>A theory test.</summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public static void TheoryTest(int a, int b)
        {
            Assert.Equal(a, b);
        }

        /// <summary>A failing theory test.</summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        [Theory(Skip = "Intentional failure for framework verification")]
        [InlineData(1, 2)]
        public static void FailedTheoryTest(int a, int b)
        {
            Assert.Equal(a, b);
        }

        /// <summary>Test exception throwing.</summary>
        [Fact]
        public static void ThrowsTest()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => throw new InvalidOperationException("Boom"));
            Assert.Equal("Boom", ex.Message);
        }

        /// <summary>Test async exception throwing.</summary>
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
