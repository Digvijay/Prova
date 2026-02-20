# Timeouts

Prova allows you to specify a maximum execution time for individual tests using the `[Timeout]` attribute. If a test exceeds this limit, it is automatically cancelled and marked as failed with a `TimeoutException`.

## Usage

Apply the `[Timeout]` attribute to a test method and specify the duration in milliseconds.

```csharp
using Prova;
using System.Threading.Tasks;

public class MyTests
{
    [Fact]
    [Timeout(1000)] // 1 second timeout
    public async Task FastTest()
    {
        await Task.Delay(100); // Passes
    }

    [Fact]
    [Timeout(50)] // 50ms timeout
    public async Task SlowTest()
    {
        await Task.Delay(100); // Fails
    }
}
```

## How it works

Prova handles timeouts differently for synchronous and asynchronous tests:

### Asynchronous Tests
For `async Task` tests, Prova uses `Task.WhenAny` with a delay task. This ensures the test is cancelled even if it doesn't support `CancellationToken` (though supporting it is still recommended for resource cleanup).

### Synchronous Tests
For synchronous `void` tests, Prova runs the test on a separate thread and monitors it from the main execution loop. If the timeout is reached, the test runner move on, and the test will eventually fail.

> [!IMPORTANT]
> Timeouts are enforced at the runner level. If a test gets stuck in an infinite loop without yielding, it might still consume resources until the process terminates, but the test result will be recorded as a failure.

## Global Timeouts
Currently, timeouts are specified per-test. Global timeout support is planned for a future release.
