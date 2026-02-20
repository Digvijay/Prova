# Dynamic Tests

Dynamic Tests allow you to generate test cases imperatively at runtime. This is ideal for scenarios where the test suite depends on external factors like files, database records, or environment configurations that are not easily expressed using static attributes.

## `[TestFactory]` Attribute

To register dynamic tests, create a `public static` method and decorate it with the `[TestFactory]` attribute. This method must accept a `DynamicTestBuilder` as its first parameter.

```csharp
using Prova;

public static class MyTestFactories
{
    [TestFactory]
    public static void CreateTests(DynamicTestBuilder builder)
    {
        var inputs = new[] { "apple", "banana", "cherry" };
        
        foreach (var input in inputs)
        {
            builder.Add($"Validate_{input}", () => 
            {
                // Test logic here
                Assert.NotNull(input);
            })
            .WithDescription($"Validates the string: {input}")
            .WithProperty("Category", "Dynamic");
        }
    }
}
```

## `DynamicTestBuilder` API

The `DynamicTestBuilder` provides a fluent API for adding and configuring tests.

### Adding Tests

- `Add(string name, Action logic)`: Adds a synchronous test.
- `Add(string name, Func<Task> logic)`: Adds an asynchronous test.
- `Add(ProvaTest test)`: Adds a pre-configured `ProvaTest` instance.

### Configuring Tests

The `Add` methods return a `TestConfigurator` that supports:

| Method | Description |
| :--- | :--- |
| `WithDescription(string)` | Sets the test description. |
| `WithSkip(string)` | Skips the test with a reason. |
| `WithRetry(int)` | Sets the number of retries. |
| `WithRepeat(int)` | Sets the number of repetitions. |
| `WithTimeout(int)` | Sets the timeout in milliseconds. |
| `WithCulture(string)` | Sets the specific culture for the test. |
| `WithParallelGroup(string)` | Assigns the test to a parallel group. |
| `WithProperty(string, string)` | Adds a custom property/trait. |
| `DoNotParallelize()` | Forces the test to run sequentially. |

## Why use Dynamic Tests?

While `[Theory]` and `[ClassData]` are great for most data-driven scenarios, Dynamic Tests offer:
- **Full control**: Use complex logic to determine what tests should exist.
- **Lazy Evaluation**: Useful if generating the test list itself is expensive.
- **Integration**: Easily bridge Prova with other systems or legacy test definitions.

> [!IMPORTANT]
> Since Prova uses a source generator, it needs to know about your library. Ensure your `[TestFactory]` methods are reachable and that the project containing them is part of the test assembly or referenced correctly.
