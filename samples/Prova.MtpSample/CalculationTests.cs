using Prova;

namespace Prova.Sample.Tests
{
    /// <summary>
    /// Tests for the Calculations service.
    /// </summary>
    public class CalculationTests
    {
        /// <summary>Tests basic addition.</summary>
        [Fact]
        [Description("Verifies basic addition")]
        public static void BasicAddition()
        {
            Assert.Equal(4, Calculations.Add(2, 2));
        }

        /// <summary>Tests addition theory.</summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="expected">Expected sum.</param>
        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(10, 20, 30)]
        [Description("Verifies addition with multiple data sets")]
        public static void AdditionTheory(int a, int b, int expected)
        {
            Assert.Equal(expected, Calculations.Add(a, b));
        }

        /// <summary>Tests async division.</summary>
        [Fact]
        [Description("Verifies async division")]
        public static async Task DivisionAsync()
        {
            var result = await Calculations.DivideAsync(10, 2);
            Assert.Equal(5, result);
        }

        /// <summary>Tests flaky behavior.</summary>
        [Fact]
        [Retry(3)]
        [Description("A test that is marked as flaky and will be retried")]
        public static void FlakyTest()
        {
            if (Random.Shared.Next(0, 2) == 0)
            {
                throw new InvalidOperationException("Random failure for retry demonstration");
            }
        }
    }
}
