# Test Context

`TestContext` provides runtime access to the currently executing test's metadata, environment settings, and execution control tokens. It uses `AsyncLocal<T>` to ensure the context flows correctly through asynchronous operations.

## Accessing the Context

You can access the current context via the static `TestContext.Current` property.

```csharp
using Prova;

public class MyTests
{
    [Fact]
    public void MyTest()
    {
        var context = TestContext.Current;
        Console.WriteLine($"Executing: {context.DisplayName}");
    }
}
```

> [!CAUTION]
> Accessing `TestContext.Current` outside the scope of a running test will throw an `InvalidOperationException`.

## Key Properties

| Property | Description |
| :--- | :--- |
| `DisplayName` | The full name of the test as displayed in reports. |
| `Properties` | A read-only dictionary of all attributes applied to the test (e.g., `[Property]`, `[Category]`). |
| `CancellationToken` | A token that is cancelled when the test times out or is manually aborted. |
| `Items` | A concurrent dictionary for storing temporary state during the test lifecycle. |

## Timeout and Cancellation

The `CancellationToken` in `TestContext` is automatically linked to the test's timeout (if specified via `[Timeout]`). This allows your test code to react gracefully to timeouts.

```csharp
[Fact]
[Timeout(2000)]
public async Task LongRunningTest()
{
    var ct = TestContext.Current.CancellationToken;
    await MyService.DoWorkAsync(ct); // Will cancel after 2 seconds
}
```

## State Sharing with `Items`

The `Items` collection is a thread-safe dictionary that exists only for the duration of a single test execution. It is useful for passing data between test hooks or storing temporary objects.

```csharp
[Before]
public void Setup()
{
    TestContext.Current.Items["DbConnection"] = OpenConnection();
}

[Fact]
public void Test()
{
    var conn = (DbConnection)TestContext.Current.Items["DbConnection"];
    // ...
}
```

## Implementation Details

`TestContext` is managed by the Prova execution engine. For each test attempt:
1. A new `TestContext` is instantiated.
2. `TestContext.Current` is set using an `AsyncLocal<T>`.
3. The test and its associated hooks are executed.
4. `TestContext.Current` is cleared.

This ensures that even in highly parallel environments, each test has isolated access to its own context.
