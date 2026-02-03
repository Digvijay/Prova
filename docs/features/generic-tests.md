# Generic Test Support

Prova provides first-class support for generic test classes and methods. To ensure compatibility with **Ahead-of-Time (AOT)** compilation, Prova uses a compile-time expansion strategy via the `[Variant]` attribute.

## Basic Usage

To define a generic test, use the `[Variant]` attribute to specify the concrete types that should be used to expand the test during compilation.

### Generic Methods

You can apply `[Variant]` to a generic method to generate multiple versions of that test.

```csharp
public class AlgorithmTests
{
    [Fact]
    [Variant(typeof(int))]
    [Variant(typeof(double))]
    public void Sorting_Works<T>() where T : IComparable<T>
    {
        T[] data = GetTestData<T>();
        Array.Sort(data);
        Assert.True(IsSorted(data));
    }
}
```

### Generic Classes

You can also apply `[Variant]` to a generic class. All test methods within the class will be expanded for each variant.

```csharp
[Variant(typeof(string))]
[Variant(typeof(int))]
public class GenericStorageTests<T>
{
    [Fact]
    public void Storage_Can_Handle_Type()
    {
        var storage = new Storage<T>();
        T value = default!;
        storage.Save(value);
        Assert.Equal(value, storage.Load());
    }
}
```

## Advanced Scenarios

### Multi-Variant Tests (Cartesian Product)

If both the class and the method have variants, Prova generates a **Cartesian Product** of all combinations.

```csharp
[Variant(typeof(string), typeof(int))]
public class ConverterTests<TInput, TOutput>
{
    [Fact]
    [Variant(typeof(JsonFormatter), typeof(XmlFormatter))]
    public void Conversion_Works<TFormatter>()
    {
        // This will generate 4 tests:
        // 1. ConverterTests<string, int>.Conversion_Works<JsonFormatter>
        // 2. ConverterTests<string, int>.Conversion_Works<XmlFormatter>
        // 3. ConverterTests<int, string>.Conversion_Works<JsonFormatter>
        // 4. ConverterTests<int, string>.Conversion_Works<XmlFormatter>
    }
}
```

### Mixed Generic and Data-Driven Tests

You can combine `[Variant]` with `[Theory]` and `[InlineData]` or `[MemberData]`.

```csharp
public class MixedTests
{
    [Theory]
    [Variant(typeof(int))]
    [Variant(typeof(string))]
    [InlineData(1)]
    [InlineData(2)]
    public void Generic_Theory<T>(int value)
    {
        Console.WriteLine($"Type: {typeof(T).Name}, Value: {value}");
    }
}
```

## How it Works

1. **Syntax Analysis**: Prova detects the `[Variant]` attributes and identifies the type parameters of the class and method.
2. **Expansion**: During source generation, Prova generates a concrete non-generic wrapper for each combination of types.
3. **Registration**: Each expanded version is registered as an independent test in the runner, ensuring they can run in parallel and report results separately.
4. **Parameter Substitution**: If a test method has parameters (e.g., in a Theory), Prova automatically substitutes any generic types with the concrete variant types to ensure correct type casting at runtime.

## Benefits

- **AOT Safe**: No runtime reflection or `MakeGenericType` / `MakeGenericMethod` calls. Everything is resolved at compile time.
- **Full Parallelism**: Different variants of the same generic test can run in parallel.
- **Aesthetic Result Reporting**: Each variant appears as a distinct test in the output (e.g., `AlgorithmTests.Sorting_Works<int>`).
