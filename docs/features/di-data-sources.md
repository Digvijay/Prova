# Dependency Injection Data Sources

DI Data Sources allow you to source test data from types registered in the Dependency Injection container. This is particularly useful when your data providers have their own dependencies or require complex initialization.

## Usage

Use the `[DependencyInjectionDataSource]` attribute on a test method or a test class.

### Method-Level DI Data Source

```csharp
public class MyDataProvider
{
    private readonly IMyService _service;
    public MyDataProvider(IMyService service) => _service = service;

    public IEnumerable<object[]> GetData() 
    {
        yield return new object[] { _service.GetPrefix() + "1" };
    }
}

public class MyTests
{
    [ConfigureServices]
    public static void Configure(ProvaServiceProvider services) 
    {
        services.Add<IMyService, MyService>();
        services.Add<MyDataProvider>();
    }

    [Theory]
    [DependencyInjectionDataSource(typeof(MyDataProvider), nameof(MyDataProvider.GetData))]
    public void Test(string value)
    {
        Assert.NotNull(value);
    }
}
```

### Class-Level DI Data Source

When applied to a class, the data source provides data for the class constructor.

```csharp
[DependencyInjectionDataSource(typeof(MyClassDataProvider))]
public class MyTests
{
    public MyTests(string data) 
    {
        // Constructor injection from DI data source
    }

    [Fact]
    public void Test1() { }
}
```

## Parameter Resolution

If the data-providing method itself has parameters, Prova will attempt to resolve them from the DI container automatically.

```csharp
public class DataProvider
{
    public IEnumerable<object[]> GetData(IMyService service) 
    {
        // 'service' is resolved from DI
        yield return new object[] { service.GetValue() };
    }
}
```

## Registration

The data provider type must be registered in the `ProvaServiceProvider` within a method decorated with `[ConfigureServices]`.
