# Test Events

Prova provides a structured event system that allows you to observe test lifecycle events (Start, End) independently of the test reporter. This is useful for custom logging, analytics, or performance monitoring.

## Interfaces

The event system uses the following interfaces, located in the `Prova` namespace:

### `ITestEventReceiver`
The base marker interface for all event receivers.

### `ITestStartEventReceiver`
Implement this interface to observe when a test is about to start.

```csharp
public interface ITestStartEventReceiver : ITestEventReceiver
{
    Task OnTestStartAsync(ProvaTest test);
}
```

### `ITestEndEventReceiver`
Implement this interface to observe when a test has completed.

```csharp
public interface ITestEndEventReceiver : ITestEventReceiver
{
    Task OnTestEndAsync(ProvaTest test, TestResult result, long durationMs);
}
```

## Global Event Registry

To observe events, you must register your receiver with the `EventRegistry`. Registration is typically done during application startup or within a `[ConfigureServices]` method.

```csharp
using Prova;

public class MyEventReceiver : ITestStartEventReceiver, ITestEndEventReceiver
{
    public Task OnTestStartAsync(ProvaTest test)
    {
        Console.WriteLine($"[START] {test.DisplayName}");
        return Task.CompletedTask;
    }

    public Task OnTestEndAsync(ProvaTest test, TestResult result, long durationMs)
    {
        Console.WriteLine($"[END] {test.DisplayName} - {result} ({durationMs}ms)");
        return Task.CompletedTask;
    }
}

// Registration
EventRegistry.Register(new MyEventReceiver());
```

## Standard Integration

The `EventRegistry` is automatically integrated into:
- **Hybrid MTP Adapter**: Events are dispatched when running tests via `dotnet test`.
- **Source Generated Runner**: Calls to the event registry are injected into the generated `RunTestSafe` method for standalone execution.

## Thread Safety

The `EventRegistry` is thread-safe and supports concurrent dispatching. It uses internal locking to ensure that registration and unregistration do not interfere with active test runs.

## Example

You can find a complete example in the `Prova.Demo` project under `EventSample.cs`.

```csharp
[ConfigureServices]
public void Configure(IServiceCollection services)
{
    EventRegistry.Register(new EventSample());
}
```
