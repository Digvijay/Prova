# Dependency Injection

Prova features a built-in, **zero-reflection** dependency injection system designed for speed and Native AOT compatibility.

## Concept

Unlike other frameworks that rely on `Microsoft.Extensions.DependencyInjection` (which is heavy on reflection), Prova uses a lightweight, source-generated approach. You register services using explicit factory delegates, and the test runner automatically wires them into your test constructors.

## Quick Start

### 1. Define Services

```csharp
public interface ICalculator { int Add(int a, int b); }
public class Calculator : ICalculator 
{ 
    public int Add(int a, int b) => a + b; 
}
```

### 2. Configure dependencies

Create a static method marked with `[ConfigureServices]`. This is your composition root.

```csharp
using Prova.Core;
using Prova.Core.Attributes;

public static class Setup
{
    [ConfigureServices]
    public static void Configure(ProvaServiceProvider services)
    {
        // Transient: Created new every time
        services.AddTransient<ICalculator>(() => new Calculator());

        // Singleton: Created once (Lazy)
        services.AddSingleton<IDatabase>(() => new InMemoryDatabase());
    }
}
```

### 3. Inject into Tests

Simply request the dependencies in your test constructor.

```csharp
public class MathTests
{
    private readonly ICalculator _calculator;

    public MathTests(ICalculator calculator)
    {
        _calculator = calculator;
    }

    [Fact]
    public void CanAdd()
    {
        Assert.Equal(4, _calculator.Add(2, 2));
    }
}
```

## Service Lifetimes

| Lifetime | Method | Description |
| :--- | :--- | :--- |
| **Transient** | `AddTransient<T>(Func<T>)` | A new instance is created every time it is injected. |
| **Singleton** | `AddSingleton<T>(Func<T>)` | A single instance is created the first time it is requested and reused for all subsequent tests. |

## FAQ

**Q: Can I use `Microsoft.Extensions.DependencyInjection`?**
A: Not directly in the core test runner, because Prova prioritizes AOT support. However, you can register a wrapper service that talks to an `IServiceProvider` if you really need to bridge the two worlds.

**Q: Does it support Scoped services (per-test)?**
A: Currently, only Singleton and Transient are supported. Scoped support is planned for a future release.
