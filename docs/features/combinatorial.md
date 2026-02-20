# 🧮 Combinatorial Testing

Combinatorial testing allows you to test a method with every possible combination of input values. This is achieved using the `[Matrix]` attribute on test parameters.

## Usage

Apply `[Matrix]` to one or more parameters of a `[Fact]` test method. Prova will automatically generate a test case for every combination (Cartesian product) of the provided values.

```csharp
[Fact]
public void TestCombinations(
    [Matrix(1, 2, 3)] int x, 
    [Matrix("a", "b")] string y)
{
    // Runs 3 x 2 = 6 times:
    // (1, "a"), (1, "b")
    // (2, "a"), (2, "b")
    // (3, "a"), (3, "b")
}
```

## Comparisons

- **[InlineData]**: Specifies exact rows of data. Good when specific combinations matter or others are invalid.
- **[Matrix]**: Specifies domains for each parameter. Good when *all* combinations are valid and should be tested.

## Supported Types
You can use any constant value supported by attributes:
- Primitives (`int`, `bool`, `double`, etc.)
- `string`
- `enums`
- `typeof(T)`
