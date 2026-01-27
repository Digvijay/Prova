using System;
using Prova;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Tests for Traits functionality.
    /// </summary>
    public class TraitTests
    {
        /// <summary>A skipped test.</summary>
        [Fact(Skip = "This test is skipped intentionally")]
        public static void SkippedTest()
        {
            Assert.True(false, "Should not run");
        }

        /// <summary>An integration test trait.</summary>
        [Fact]
        [Trait("Category", "Integration")]
        public static void IntegrationTest()
        {
            Assert.True(true);
        }

        /// <summary>A unit test trait.</summary>
        [Fact]
        [Trait("Category", "Unit")]
        public static void UnitTest()
        {
            Assert.True(true);
        }
    }

    /// <summary>
    /// Slow tests.
    /// </summary>
    [Trait("Category", "Slow")]
    public class SlowTests
    {
        /// <summary>A heavy workload test.</summary>
        [Fact]
        public static async Task HeavyWork()
        {
            await Task.Delay(100);
            Assert.True(true);
        }
    }
}
