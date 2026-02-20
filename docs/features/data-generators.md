# Data Generators

Prova allows you to create custom, reusable data sources by inheriting from the `DataSourceGeneratorAttribute` class. This is similar to xUnit's `DataAttribute` and enables complex data generation logic that can be shared across multiple tests.

## Creating a Custom Data Generator

To create a custom data generator, inherit from `DataSourceGeneratorAttribute` and implement the `GetData()` method. This method should return an `IEnumerable<object?[]>`, where each `object?[]` represents a row of data for your test method.

### Simple Range Generator

```csharp
using Prova;
using System.Collections.Generic;

public class RangeDataAttribute : DataSourceGeneratorAttribute
{
    private readonly int _start;
    private readonly int _count;

    public RangeDataAttribute(int start, int count)
    {
        _start = start;
        _count = count;
    }

    public override IEnumerable<object?[]> GetData()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return new object?[] { _start + i };
        }
    }
}
```

## Using Custom Data Generators

You can apply your custom attribute to any test method marked with `[Theory]`.

```csharp
public class MyTests
{
    [Theory]
    [RangeData(1, 5)]
    public void Test_Range(int value)
    {
        Assert.True(value >= 1 && value <= 5);
    }
}
```

### Multiple Generators

You can apply multiple custom generators to the same test method. Prova will execute the test for each row provided by each generator.

```csharp
[Theory]
[RangeData(1, 3)]
[RangeData(10, 2)]
public void Test_MixedRanges(int value)
{
    // Executes for 1, 2, 3, 10, 11
}
```

## Class-Level Data Generators

Custom data generators can also be applied at the class level to provide data for the class constructor.

```csharp
[RangeData(100, 2)]
public class MyClassTests
{
    private readonly int _data;

    public MyClassTests(int data)
    {
        _data = data;
    }

    [Fact]
    public void Test()
    {
        Assert.True(_data == 100 || _data == 101);
    }
}
```

### Combining Class and Method Data

If both class-level and method-level data sources are used, Prova will generate a Cartesian product of the data rows.

```csharp
[RangeData(1, 2)] // Class-level
public class CombinedTests
{
    public CombinedTests(int classData) { ... }

    [Theory]
    [InlineData(""A"")]
    [InlineData(""B"")]
    public void Test(string methodData)
    {
        // Executes:
        // (1, ""A"")
        // (1, ""B"")
        // (2, ""A"")
        // (2, ""B"")
    }
}
```

## Skipping Data Rows

You can use the `Skip` property on your custom attribute to skip all data rows provided by that generator.

```csharp
[Theory]
[RangeData(1, 5, Skip = ""Temporarily disabled"")]
public void Test_Skipped(int value) { ... }
```
