using Prova;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class DataDrivenTests
    {
        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(10, 20, 30)]
        public void InlineDataTest(int a, int b, int expected)
        {
            Assert.Equal(expected, a + b);
        }

        // 1. Sourcing from a static property
        public static IEnumerable<object[]> SimpleData => new List<object[]>
        {
            new object[] { 1, 2, 3 },
            new object[] { 10, 20, 30 }
        };

        [Theory]
        [MemberData(nameof(SimpleData))]
        public void AddTest(int a, int b, int expected)
        {
            Assert.Equal(expected, a + b);
        }

        // 2. Sourcing from a static method
        public static IEnumerable<object[]> GetDynamicData(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new object[] { i, i * 2 };
            }
        }

        [Theory]
        [MemberData(nameof(GetDynamicData), 3)]
        public void MethodDataSourceTest(int input, int expected)
        {
            Assert.Equal(expected, input * 2);
        }

        // 3. Sourcing from an external class
        [Theory]
        [MemberData(nameof(ExternalDataProvider.ExternalData), MemberType = typeof(ExternalDataProvider))]
        public void ExternalDataSourceTest(string input, int length)
        {
            Assert.Equal(length, input.Length);
        }
    }

    public static class ExternalDataProvider
    {
        public static IEnumerable<object[]> ExternalData => new List<object[]>
        {
            new object[] { "Prova", 5 },
            // new object[] { "AOT", 3 }
        };
    }
}
