using System.Collections.Generic;
using Prova;

namespace Prova.Core.Tests
{
    public class MemberDataTests
    {
        public static IEnumerable<object[]> TestData => new List<object[]>
        {
            new object[] { 1, 2, 3 },
            new object[] { 5, 5, 10 }
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public static void Add(int a, int b, int expected)
        {
            Assert.Equal(expected, a + b);
        }
    }
}
