using System.Collections.Generic;
using Prova;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Tests for MemberData feature.
    /// </summary>
    public class MemberDataTests
    {
        /// <summary>Gets test data.</summary>
        public static IEnumerable<object[]> TestData => new List<object[]>
        {
            new object[] { 1, 2, 3 },
            new object[] { 5, 5, 10 }
        };

        /// <summary>Adds numbers.</summary>
        /// <param name="a">First number.</param>
        /// <param name="b">Second number.</param>
        /// <param name="expected">Expected sum.</param>
        [Theory]
        [MemberData(nameof(TestData))]
        public static void Add(int a, int b, int expected)
        {
            Assert.Equal(expected, a + b);
        }
    }
}
