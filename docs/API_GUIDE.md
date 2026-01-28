# Prova API Guide

This guide covers the entire feature set of the Prova testing framework, from basic assertions to advanced enterprise governance tools.

---

## Core Features

### Writing a Simple Test
The `[Fact]` attribute marks a method as a test. It must be `public` and return `void` or `Task`.

```csharp
public class MathTests
{
    [Fact]
    public void Addition_Works()
    {
        int result = 2 + 2;
        Assert.Equal(4, result);
    }

    [Fact] // Async tests are fully supported
    public async Task Async_Operation_Works()
    {
        await Task.Delay(10);
        Assert.True(true);
    }
}
```

### Assertions
Prova includes a standard suite of assertions similar to xUnit.

```csharp
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);
Assert.True(condition);
Assert.False(condition);
Assert.Null(obj);
Assert.NotNull(obj);
Assert.Same(expectedRef, actualRef);
Assert.NotSame(expectedRef, actualRef);
Assert.Contains("sub", "substring");
Assert.Empty(collection);
Assert.Single(collection);
Assert.Throws<InvalidOperationException>(() => { ... });
```

---

## Intermediate Features

### Data-Driven Tests (`[Theory]`)
Use `[Theory]` to run the same test logic with multiple inputs.

#### Inline Data
Simple values passed directly.
```csharp
[Theory]
[InlineData(1, 2, 3)]
[InlineData(10, 20, 30)]
public void Add_Works(int a, int b, int expected)
{
    Assert.Equal(expected, a + b);
}
```

#### Member Data
Load data from a static property or method (returns `IEnumerable<object[]>`).
```csharp
public static IEnumerable<object[]> GetIndices => 
    new List<object[]> { new object[] { 1 }, new object[] { 2 } };

[Theory]
[MemberData(nameof(GetIndices))]
public void Test_With_MemberData(int index) { ... }
```

#### Class Data
Load data from a separate class (implements `IEnumerable<object[]>`).
```csharp
public class TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator() 
    {
        yield return new object[] { "A" }; 
        yield return new object[] { "B" };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassData(typeof(TestData))]
public void Test_With_ClassData(string letter) { ... }
```

### Lifecycle (`IAsyncLifetime`)
Prova does **not** use `SetUp` or `TearDown` attributes. Instead, it uses standard .NET interfaces.
- **Constructor**: Runs before every test.
- **Dispose**: Runs after every test.
- **IAsyncLifetime**: For async setup/teardown.

```csharp
public class DatabaseTests : IAsyncLifetime
{
    private readonly Database _db;

    public Task InitializeAsync()
    {
        _db = await Database.ConnectAsync();
    }

    [Fact]
    public void TestQuery() => ...

    public Task DisposeAsync()
    {
        return _db.CloseAsync();
    }
}
```

### Output Capture
To log messages that appear in the test report (and Visual Studio output window), inject `ITestOutputHelper`.

```csharp
public class LogTests
{
    private readonly ITestOutputHelper _output;

    public LogTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test_With_Logs()
    {
        _output.WriteLine("Step 1: Connecting...");
        // This log is attached to the specific test result
    }
}
```

---

## Advanced Features

### Explicit Concurrency (`[Parallel]`)
Controls how many tests in a class run simultaneously.

```csharp
[Parallel(max: 2)] // Only 2 tests in this class will run at once
public class ResourceHeavyTests
{
    [Fact] public async Task Test1() { ... }
    [Fact] public async Task Test2() { ... }
    [Fact] public async Task Test3() { ... } // Waits for 1 or 2
}
```

### Allocation Budget (`[MaxAlloc]`)
Enforce strict memory usage policies. Useful for "hot paths" in high-performance code.

```csharp
[Fact]
[MaxAlloc(0)] // zero allocations allowed!
public void Parse_Span_ShouldNotAllocate()
{
    int.Parse("123".AsSpan()); // Safe
    // int.Parse("123"); // Fails (allocates string)
}

[Fact]
[MaxAlloc(1024)] // 1KB budget
public void Moderate_Allocation_Test() { ... }
```

### Timeouts (`[Timeout]`)
Prevent CI hangs by enforcing execution limits.

```csharp
[Fact]
[Timeout(500)] // Fails if > 500ms
public async Task Network_Request_Must_Be_Fast() { ... }
```

### Retry Logic (`[Retry]`)
Automatically retry known flaky tests (e.g., integration tests accessing unreliable external services).

```csharp
[Fact]
[Retry(3)] // Retries up to 3 times before failing
public void Unreliable_Service_Call() { ... }
```

### Dependency Injection (`[TestDependency]`)
A Compile-time, AOT-safe dependency injection system.
1. Define a `static` factory method with `[TestDependency]`.
2. Add the dependency to your test class constructor.

```csharp
public static class Factories
{
    [TestDependency]
    public static ILogger CreateLogger() => new ConsoleLogger();
}

public class MyTests
{
    private readonly ILogger _logger;
    
    // Prova finds 'CreateLogger' automatically and injects it here
    public MyTests(ILogger logger)
    {
        _logger = logger;
    }

    [Fact]
    public void Test_Logging() 
    {
        _logger.Log("Hello");
    }
}
```

### Focus Mode (`[Focus]`)
Similar to `.only` in other frameworks. When `[Focus]` is present on ANY test, **only** those tests run. All others are skipped (and their code isn't even generated/compiled in the runtime path).

```csharp
[Fact]
[Focus] // <--- I am debugging this!
public void Debugging_Test() 
{
    // ...
}
```
