using System.Threading.Tasks;
using Prova;

// Run tests
return await Prova.TestRunnerExecutor.RunAllAsync(args);

namespace VariantSample
{
    public class VariantTests
    {
        [Fact]
        [TestVariant("Red")]
        [TestVariant("Blue")]
        public async Task ShouldHaveCorrectVariant()
        {
            var variant = TestContext.Current.Variant;
            Assert.NotNull(variant);
            Assert.True(variant == "Red" || variant == "Blue");
            await Task.CompletedTask;
        }

        [Fact]
        public async Task ShouldHaveNoVariant()
        {
            var variant = TestContext.Current.Variant;
            Assert.Null(variant);
            await Task.CompletedTask;
        }
    }
}
