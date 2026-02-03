# Test Variants

Test Variants allow you to execute the same test method multiple times under different named scenarios, without the complexity of parameterized tests (`[Theory]`) when you only need a scenario name.

## Usage

Use the `[TestVariant("VariantName")]` attribute on your test method. You can apply it multiple times.

```csharp
using Prova;
using System.Threading.Tasks;

public class MyTests
{
    [TestVariant("HTTP/1.1")]
    [TestVariant("HTTP/2")]
    [TestVariant("HTTP/3")]
    public async Task FeatureCheck()
    {
        var protocol = TestContext.Current.Variant;
        
        // Use the variant to configure your test logic
        var client = CreateClient(protocol);
        
        await client.GetAsync("/");
    }
}
```

## Accessing the Variant

The current variant name is available via the `TestContext`:

```csharp
string? variant = TestContext.Current.Variant;
```

If the test is running without a variant (e.g. standard `[Fact]`), this property returns `null`.

## Execution

When you run your tests, Prova creates a unique test case for each variant. Examples:

- `MyTests.FeatureCheck [Variant: HTTP/1.1]`
- `MyTests.FeatureCheck [Variant: HTTP/2]`
- `MyTests.FeatureCheck [Variant: HTTP/3]`

## Combined with Other Features

Test Variants work seamlessly with other Prova features:

- **Lifecycle Hooks**: `[Before]`/`[After]` hooks run for *each* variant execution.
- **Parallel Execution**: Variants run in parallel by default (unless restricted).
- **Data Driven Tests**: If you combine `[TestVariant]` with `[InlineData]`, it generates a combinatorial set of tests (Variant * DataRow).
