# Data Sources

Prova supports advanced data-driven testing through `[ClassDataSource]` and `[MethodDataSource]`. Unlike standard data attributes, these are **DI-aware**, meaning the data providers themselves can be resolved from the service provider and even take dependencies in their methods.

## [ClassDataSource]

Use `[ClassDataSource]` to provide test data from a class that implements `IEnumerable<object[]>`. The class is resolved via Dependency Injection.

### Method Level
When applied to a method, the data class provides parameters for that specific method.

```csharp
public class MyDataSource : IEnumerable<object[]> 
{
    // ...
}

[Theory]
[ClassDataSource(typeof(MyDataSource))]
public void Test(int x, string s) { }
```

### Class Level
When applied to a class, the data class provides parameters for the **test class constructor**. This effectively runs every test in the class for each row of data provided.

```csharp
[ClassDataSource(typeof(MyDataSource))]
public class MyTests 
{
    public MyTests(int x, string s) { }

    [Fact]
    public void Test() { }
}
```

## [MethodDataSource]

Use `[MethodDataSource]` to provide test data from a method (static or instance). If the method is an instance method, Prova will resolve the containing class from the service provider.

The data provider method can itself take parameters resolved from DI.

```csharp
public class UserProvider 
{
    private readonly IDatabase _db;
    public UserProvider(IDatabase db) => _db = db;

    public IEnumerable<object[]> GetUsers(ILogger logger) 
    {
        // Resolve parameters like 'logger' from DI
        yield return new object[] { 1, "Alice" };
    }
}

[Theory]
[MethodDataSource(nameof(UserProvider.GetUsers), MemberType = typeof(UserProvider))]
public void Test(int id, string name) { }
```

## Configuration

Ensure your data providers are registered in your test class using `[ConfigureServices]`.

```csharp
[ConfigureServices]
public static void Configure(ProvaServiceCollection services)
{
    services.Add<UserProvider>();
    services.Add<MyDataSource>();
}
```

## AOT Safety

Like all Prova features, Data Sources use compile-time source generation and are fully compatible with Native AOT.
