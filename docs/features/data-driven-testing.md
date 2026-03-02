# 📊 Data-Driven Testing

Prova supports powerful data-driven testing using `[Theory]` combined with various data source attributes. This allows you to run the same test logic multiple times with different inputs.

## 📥 Inline Data
The simplest way to provide data is using `[InlineData]`.

```csharp
using Prova;

public class MyTests
{
    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(10, 20, 30)]
    public void AddTest(int a, int b, int expected)
    {
        Assert.Equal(expected, a + b);
    }
}
```

## 🏗️ Class Data
For complex data or data that needs to be shared across multiple test classes, use `[ClassData]`. The specified class must implement `IEnumerable<object[]>`.

```csharp
public class MyTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1, "One" };
        yield return new object[] { 2, "Two" };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassData(typeof(MyTestData))]
public void SharedDataTest(int id, string name) { ... }
```

## 🔗 Member Data
Use `[MemberData]` to source data from a static property, method, or field. This is highly flexible and supports parameters for methods.

### Static Property
```csharp
public static IEnumerable<object[]> PropertyData => new[]
{
    new object[] { 1 },
    new object[] { 2 }
};

[Theory]
[MemberData(nameof(PropertyData))]
public void PropertyTest(int x) { ... }
```

### Static Method with Parameters
```csharp
public static IEnumerable<object[]> GetMethodData(int count)
{
    for (int i = 0; i < count; i++) yield return new object[] { i };
}

[Theory]
[MemberData(nameof(GetMethodData), 3)]
public void MethodTest(int x) { ... }
```

### External Member
You can also source data from a member in a different class using the `MemberType` property.

```csharp
[Theory]
[MemberData(nameof(ExternalSource.Data), MemberType = typeof(ExternalSource))]
public void ExternalTest(int x) { ... }
```

## 🏢 Class-Level Data (Constructor Parameterization)

Prova allows you to apply `[InlineData]`, `[MemberData]`, or `[ClassData]` directly to a test class. When done, every test in the class will be executed for each row of data provided, and the data will be injected into the class constructor.

This is extremely useful for testing a component with multiple configurations (e.g., different storage providers, different mock settings).

```csharp
[InlineData("StorageA", 1024)]
[InlineData("StorageB", 2048)]
public class StorageTests
{
    private readonly string _name;
    private readonly int _limit;

    public StorageTests(string name, int limit)
    {
        _name = name;
        _limit = limit;
    }

    [Fact]
    public void VerifyLimit()
    {
        Assert.Equal(_limit, GetStorage(_name).Capacity);
    }
}
```

> [!TIP]
> You can combine Class-Level Data with Method-Level Theories. If a class has 2 data rows and a method has 3 data rows, the method will run a total of 6 times (2 * 3).

## 🚀 Performance Note: Theory Unrolling
Because Prova is a source-generated runner, `[Theory]` + `[InlineData]` combinations are subject to **Theory Unrolling** (introduced in v0.5.0). 

Each `[InlineData]` row is explicitly "unrolled" into a separate, statically generated, parameterless test method at compile-time. This guarantees:
- **Zero Reflection at runtime**
- **Zero Boxed array allocations (`object[]`)**
- **Full Native AOT compatibility**

For dynamic data sources like `[MemberData]` or `[ClassData]`, Prova generates efficient runtime execution loops, heavily minimizing reflection usage compared to traditional test runners.
