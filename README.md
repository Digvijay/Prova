# Prova 🇸🇪

![Build Status](https://img.shields.io/github/actions/workflow/status/Digvijay/Prova/ci.yml?branch=master)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![NuGet](https://img.shields.io/nuget/v/Prova.svg)

**Prova** is a high-performance, Native AOT-compatible test runner for .NET. Use the xUnit syntax you already know, but with zero runtime reflection and instant startup.

## Why Prova?

### 1. Zero Migration Cost
Your tests remain exactly the same. Prova supports standard xUnit attributes (`[Fact]`, `[Theory]`, `[InlineData]`, `Assert`). You only change the runner.

### 2. Native Performance
Tests are discovered at compile-time using Source Generators and can be compiled to Native AOT. This eliminates runtime discovery costs and enables instant startup, making it ideal for high-frequency "inner loop" development or containerized CI environments.

### 3. Safe Parallelism
Tests run in parallel by default (`Task.WhenAll`), utilizing all available cores.

---

## Quick Start

### 1. Install Code
```bash
dotnet add package Prova
```

### 2. Write Tests (Standard xUnit Syntax)
```csharp
using Prova; // or 'using Xunit;' (Prova aliases this for compatibility)

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        Assert.Equal(4, 2 + 2);
    }

    [Theory]
    [InlineData(10, 2, 5)]
    [InlineData(20, 4, 5)]
    public void Divide_ReturnsQuotient(int a, int b, int expected)
    {
        Assert.Equal(expected, a / b);
    }
}
```

### 3. Run
```bash
dotnet run
```

---

## Microsoft Testing Platform (MTP) Support

Prova integrates with the [Microsoft Testing Platform](https://github.com/microsoft/testfx). This enables support for `dotnet test`, TRX reporting, and code coverage without sacrificing AOT compatibility.

**To enable `dotnet test` support:**

1. Add a `global.json` to your solution root:
   ```json
   { "test": { "runner": "Microsoft.Testing.Platform" } }
   ```

2. Run with standard tooling:
   ```bash
   dotnet test --coverage --report-trx
   ```

---

## Advanced Features

While Prova is a drop-in replacement, it adds enterprise-grade features for strictly governed codebases.

### Explicit Concurrency
Prevent thread pool starvation in massive test suites by bounding parallelism.

```csharp
[Parallel(max: 4)] // Limits concurrency for this class
public class DatabaseTests { ... }
```

### Allocation Governance
Enforce zero-allocation policies or strict memory budgets for critical paths.

```csharp
[Fact]
[MaxAlloc(0)] // Fails if the test allocates any memory on the heap
public void HotPath_ShouldNotAllocate() { ... }
```

### Flakiness Management
```csharp
[Fact]
[Retry(3)] // Automatically retry flaky network tests
public void IntegrationTest() { ... }
```

### Focused Execution
Run only the specific test you are debugging (similar to `.only` in Jest).

```csharp
[Fact]
[Focus] // Prova will ONLY generate and run this test
public void DebuggingThisRightNow() { ... }
```

---

## Contributing

We welcome issues and pull requests. Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## See it in Action

Prova produces clean, hierarchical output that is easy to parse visually.

![Test Output](docs/assets/comparison_output.png)

## License

MIT
