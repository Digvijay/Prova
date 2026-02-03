# Dependency Injection

Prova supports first-class Dependency Injection (DI) to help you write modular and testable code. It uses a lightweight, built-in service container (`ProvaServiceCollection`) that works seamlessly with your test classes.

## Features

- **Constructor Injection**: Automatically inject services into your Test Class constructor.
- **Method Injection**: Inject services directly into `[Fact]` or `[Theory]` methods (coming soon).
- **ConfigureServices**: Register services using the `[ConfigureServices]` attribute.

## Basic Usage

### 1. Register Services

Use the `[ConfigureServices]` attribute on a static method to register your dependencies. This method typically resides in a global setup class or the test class itself.

```csharp
using Prova.Core;
using Prova.Core.Attributes;

public class TestSetup
{
    [ConfigureServices]
    public static void Configure(ProvaServiceCollection services)
    {
        // Register a singleton
        services.AddSingleton<IDatabase>(() => new InMemoryDatabase());

        // Register a transient
        services.AddTransient<ICalculator>(() => new Calculator());
    }
}
```

### 2. Inject into Test Class

Simply add parameters to your Test Class constructor. Prova will automatically resolve them.

```csharp
using Prova;

public class MyServiceTests
{
    private readonly IDatabase _db;
    private readonly ICalculator _calc;

    public MyServiceTests(IDatabase db, ICalculator calc)
    {
        _db = db;
        _calc = calc;
    }

    [Fact]
    public void VerifyCalculation()
    {
        var result = _calc.Add(2, 3);
        Assert.Equal(5, result);
    }
}
```

## ProvaServiceCollection

Prova uses its own `ProvaServiceCollection` which provides a simple, zero-reflection API for registration:

- `AddSingleton<T>(Func<T> factory)`: Created once and shared.
- `AddTransient<T>(Func<T> factory)`: Created every time it is requested.
- `Get<T>()`: Manually resolve a service (useful in hooks).

> [!NOTE]
> Prova's DI is designed for speed and simplicity in testing scenarios. It does not support advanced features like open generics or auto-registration/scanning found in larger containers.
