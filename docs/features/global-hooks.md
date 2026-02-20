# Global Hooks

Global hooks run before/after **every** test or class in your entire test suite. Use them for cross-cutting concerns like logging, telemetry, or resource management.

## Quick Start

```csharp
using Prova;
using System.Threading.Tasks;

public static class GlobalSetup
{
    [BeforeEvery(HookScope.Test)]
    public static void BeforeEachTest()
    {
        Console.WriteLine("Starting a test...");
    }

    [BeforeEvery(HookScope.Class)]
    public static async Task GlobalClassSetup()
    {
        await Task.Delay(1);
    }
}
```

## Attributes

| Attribute | Scope | Description |
|-----------|-------|-------------|
| `[BeforeEvery(HookScope.Test)]` | Test | Runs before each test method |
| `[AfterEvery(HookScope.Test)]` | Test | Runs after each test method |
| `[BeforeEvery(HookScope.Class)]` | Class | Runs before the first test of each class |
| `[AfterEvery(HookScope.Class)]` | Class | Runs after the last test of each class |

## Requirements

Global hook methods must be:
- **Static** — Instance methods are not supported
- **Async friendly** — Can return `void`, `Task`, or `ValueTask`
- **Parameterless** — No dependency injection support currently

## HookScope Values

```csharp
public enum HookScope
{
    Test,     // 0 - Individual test methods
    Class,    // 1 - Test classes
    Assembly  // 2 - Entire test assembly (use [Before]/[After] instead)
}
```

## Comparison with [Before]/[After]

| Feature | `[Before]`/`[After]` | `[BeforeEvery]`/`[AfterEvery]` |
|---------|---------------------|-------------------------------|
| Scope | Single class | All classes |
| Instance | Can be instance method | Must be static |
| Use Case | Class-specific setup | Cross-cutting concerns |

## Common Use Cases

### Logging
```csharp
[BeforeEvery(HookScope.Test)]
public static void LogTestStart()
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Test starting...");
}
```

### Telemetry
```csharp
private static Stopwatch _sw;

[BeforeEvery(HookScope.Test)]
public static void StartTimer() => _sw = Stopwatch.StartNew();

[AfterEvery(HookScope.Test)]
public static void ReportDuration() 
{
    TelemetryClient.TrackMetric("TestDuration", _sw.ElapsedMilliseconds);
}
```

### Resource Cleanup
```csharp
[AfterEvery(HookScope.Class)]
public static void ResetDatabase()
{
    TestDatabase.Reset();
}
```

## AOT Compatibility

Global hooks are fully compatible with Native AOT. They're resolved at compile time by the source generator.
