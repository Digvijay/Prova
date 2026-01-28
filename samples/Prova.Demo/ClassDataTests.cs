using System.Collections;
using System.Collections.Generic;
using Prova;

namespace Prova.Demo
{
    public class NumberGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { 10, 20, 30 };
            yield return new object[] { 5, 5, 10 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ClassDataTests
    {
        [Theory]
        [ClassData(typeof(NumberGenerator))]
        public void Add_MatchesExpected(int a, int b, int expected)
        {
            Assert.Equal(expected, a + b);
        }
    }
}
