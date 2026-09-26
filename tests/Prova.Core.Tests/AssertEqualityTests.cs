using System.Collections.Generic;
using System.Linq;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Covers equality assertions.
    /// </summary>
    /// <remarks>
    /// <c>Assert.Equal</c> used to delegate straight to the default equality comparer, which
    /// compares arrays and lists by reference. Comparing two equal sequences therefore failed.
    /// There was also no <c>Assert.NotEqual</c> at all, which pushed people towards
    /// <c>Assert.False(a == b)</c> — an assertion whose failure message says nothing about what
    /// the values actually were.
    /// </remarks>
    public class AssertEqualityTests
    {
        /// <summary>Two arrays holding the same values are equal.</summary>
        [Fact]
        public void Equal_Compares_Arrays_By_Value()
        {
            var expected = new[] { 1, 2, 3 };
            var actual = new[] { 1, 2, 3 };

            Assert.Equal(expected, actual);
        }

        /// <summary>A list and an array holding the same values are equal.</summary>
        [Fact]
        public void Equal_Compares_Lists_By_Value()
        {
            var expected = new List<string> { "a", "b" };
            var actual = new[] { "a", "b" };

            Assert.Equal<IEnumerable<string>>(expected, actual);
        }

        /// <summary>Collections nested inside collections compare by value too.</summary>
        [Fact]
        public void Equal_Compares_Nested_Collections_By_Value()
        {
            var expected = new[] { new[] { 1, 2 }, new[] { 3 } };
            var actual = new[] { new[] { 1, 2 }, new[] { 3 } };

            Assert.Equal(expected, actual);
        }

        /// <summary>A shorter sequence is not equal to a longer one that shares its prefix.</summary>
        [Fact]
        public void Equal_Rejects_Sequences_Of_Different_Length()
        {
            var expected = new[] { 1, 2 };
            var actual = new[] { 1, 2, 3 };

            Assert.Throws<AssertException>(() => Assert.Equal(expected, actual));
        }

        /// <summary>Sequences of the same length with differing elements are not equal.</summary>
        [Fact]
        public void Equal_Rejects_Sequences_With_Differing_Elements()
        {
            var expected = new[] { 1, 2 };
            var actual = new[] { 1, 9 };

            Assert.Throws<AssertException>(() => Assert.Equal(expected, actual));
        }

        /// <summary>
        /// Strings are enumerable, but are still compared as strings so that the failure message
        /// stays readable.
        /// </summary>
        [Fact]
        public void Equal_Still_Compares_Strings_As_Strings()
        {
            Assert.Equal("abc", "abc");
            Assert.Throws<AssertException>(() => Assert.Equal("abc", "abd"));
        }

        /// <summary>A lazily evaluated sequence compares against a materialised one.</summary>
        [Fact]
        public void Equal_Compares_A_Lazy_Sequence_Without_Requiring_A_Materialised_One()
        {
            var actual = new[] { 1, 2, 3 };

            Assert.Equal<IEnumerable<int>>(Enumerable.Range(1, 3), actual);
        }

        /// <summary>Values that differ satisfy <c>NotEqual</c>.</summary>
        [Fact]
        public void NotEqual_Accepts_Differing_Values()
        {
            var expected = new[] { 1, 2 };
            var actual = new[] { 2, 1 };

            Assert.NotEqual(1, 2);
            Assert.NotEqual("a", "b");
            Assert.NotEqual(expected, actual);
        }

        /// <summary>Values that are equal fail <c>NotEqual</c>, collections included.</summary>
        [Fact]
        public void NotEqual_Rejects_Equal_Values()
        {
            var expected = new[] { 1, 2 };
            var actual = new[] { 1, 2 };

            Assert.Throws<AssertException>(() => Assert.NotEqual(1, 1));
            Assert.Throws<AssertException>(() => Assert.NotEqual(expected, actual));
        }

        /// <summary>
        /// A failure message names the elements. A message reading
        /// <c>Expected: System.Int32[]</c> tells the reader nothing.
        /// </summary>
        [Fact]
        public void A_Failure_Message_Shows_The_Contents_Of_A_Collection()
        {
            var expected = new[] { 1, 2 };
            var actual = new[] { 1, 9 };

            var failure = Assert.Throws<AssertException>(() => Assert.Equal(expected, actual));

            Assert.Contains("[1, 2]", failure.Message);
            Assert.Contains("[1, 9]", failure.Message);
        }

        /// <summary>Null compares equal to null and unequal to anything else.</summary>
        [Fact]
        public void Equal_Handles_Nulls()
        {
            Assert.Equal<string?>(null, null);
            Assert.Throws<AssertException>(() => Assert.Equal<string?>(null, "a"));
            Assert.Throws<AssertException>(() => Assert.Equal<string?>("a", null));
        }
    }
}
