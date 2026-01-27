using System;
using Prova;

namespace Prova.Core.Tests.Framework
{
    /// <summary>
    /// Tests demonstrating documentation features.
    /// </summary>
    public class DocsTests
    {
        /// <summary>
        /// This is a test that has documentation! 🧙‍♂️
        /// </summary>
        [Fact]
        [Trait("Category", "Magic")]
        public static void DocTest()
        {
            Assert.True(true);
        }

        private static readonly string[] Names = { "a", "b" };
        private static readonly int[] One = { 1 };

        /// <summary>Test collection assertions.</summary>
        [Fact]
        public static void CollectionTest()
        {
            Assert.Contains("a", Names);
            Assert.DoesNotContain("c", Names);
            Assert.Single(One);
            Assert.Empty(Array.Empty<int>());
        }
        
        /// <summary>Test type assertions.</summary>
        [Fact]
        public static void TypeTest()
        {
            Assert.IsType<string>("hello");
            Assert.Same("a", "a"); // String interning makes this true usually
        }
    }
}
