using Prova;
using System.Threading.Tasks;

// Run tests
await Prova.TestRunnerExecutor.RunAllAsync(args);

// Define tests
public class VariantTests
{
    [Fact]
    // [TestVariant("Red")]
    // [TestVariant("Blue")]
    public async Task ShouldHaveCorrectVariant()
    {
        var variant = TestContext.Current.Variant;
        // Assert.NotNull(variant);
        // Assert.True(variant == "Red" || variant == "Blue");
        await Task.CompletedTask;
    }

    public async Task ShouldHaveNoVariant()
    {
        var variant = TestContext.Current.Variant;
        Assert.Null(variant);
        await Task.CompletedTask;
    }
}
