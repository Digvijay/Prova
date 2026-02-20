# TestContext API 🌐

The `TestContext` class provides access to metadata and runtime information about the currently executing test. It is automatically injected into test classes that include it as a constructor parameter.

## Accessing the Context

To use `TestContext`, simply add it to your test class constructor:

```csharp
public class MyTests
{
    private readonly TestContext _context;

    public MyTests(TestContext context)
    {
        _context = context;
    }

    [Fact]
    public void Test()
    {
        var name = _context.DisplayName;
        // Use context...
    }
}
```

## Properties

| Property | Description |
|----------|-------------|
| `DisplayName` | The display name of the test (including variants). |
| `FullName` | The fully qualified name (`Namespace.Class.Method`). |
| `ClassName` | The name of the test class. |
| `MethodName` | The name of the test method. |
| `Variant` | The variant name (for theories or matrix tests). |
| `Properties` | A dictionary of all properties/traits applied to the test. |
| `State` | The **State Bag** for sharing data between lifecycle hooks. |

## The State Bag 📦

The `State` property is a `ConcurrentDictionary<string, object?>` that lives for the duration of a single test execution. It is the primary way to pass data from a `[Before]` hook to the test method and then to an `[After]` hook.

### Example

```csharp
public class MyTests
{
    private readonly TestContext _context;

    public MyTests(TestContext context) => _context = context;

    [Before]
    public void Setup()
    {
        _context.State["Timestamp"] = DateTime.UtcNow;
    }

    [Fact]
    public void Test()
    {
        var start = (DateTime)_context.State["Timestamp"];
        // ...
    }

    [After]
    public void Teardown()
    {
        var start = (DateTime)_context.State["Timestamp"];
        var duration = DateTime.UtcNow - start;
        // ...
    }
}
```
