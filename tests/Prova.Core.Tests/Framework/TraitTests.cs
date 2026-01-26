using System;
using Prova;

namespace Prova.Core.Tests
{
    public class TraitTests
    {
        [Fact(Skip = "This test is skipped intentionally")]
        public static void SkippedTest()
        {
            Assert.True(false, "Should not run");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public static void IntegrationTest()
        {
            Assert.True(true);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public static void UnitTest()
        {
            Assert.True(true);
        }
    }

    [Trait("Category", "Slow")]
    public class SlowTests
    {
        [Fact]
        public static async Task HeavyWork()
        {
            await Task.Delay(100);
            Assert.True(true);
        }
    }
}
