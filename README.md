# Prova

[![Build Status](https://img.shields.io/github/actions/workflow/status/Digvijay/Prova/ci.yml?branch=master)](https://github.com/Digvijay/Prova/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Prova.svg)](https://www.nuget.org/packages/Prova)
[![Docs](https://img.shields.io/badge/docs-prova.digvijay.dev-blue)](https://prova.digvijay.dev)

**Prova** is a high-performance, MTP-native testing framework for .NET 10. It uses the xUnit syntax you already know, but with zero runtime reflection and full Native AOT compatibility. Theories are "unrolled" at compile-time into parameterless test methods -- no boxing, no reflection, instant startup.

[Read the full documentation](https://prova.digvijay.dev)

## Why Prova?

### 1. Zero Migration Cost
Your tests remain exactly the same. Prova supports standard xUnit attributes (`[Fact]`, `[Theory]`, `[InlineData]`, `Assert`). You only change the runner.

### 2. MTP-Native Execution
Prova implements the [Microsoft Testing Platform](https://github.com/microsoft/testfx) (MTP) natively. This means full `dotnet test` support, TRX reporting, crash/hang dump collection, and code coverage -- all without a legacy VSTest bridge.

### 3. Theory Unrolling (Zero Reflection)
Each `[InlineData]` row is statically compiled into its own parameterless test method by the Source Generator. There is zero boxing, zero `object[]` allocation, and zero reflection at runtime. This makes `[Theory]` tests fully Native AOT compatible.

### 4. Self-Executing Test Projects
Prova auto-generates a `Program.cs` entry point with UTF-8 output. Your test project is a self-executing console app from `dotnet new`. No boilerplate needed.

### 5. Safe Parallelism
Tests run in parallel by default (`Task.WhenAll`), utilizing all available cores. Use `[Parallel(max: N)]` to bound concurrency.

---

## Quick Start

### 1. Create a Test Project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Prova" Version="0.5.0" />
  </ItemGroup>
</Project>
```

> **Note:** `<OutputType>Exe</OutputType>` and `<TargetFramework>net10.0</TargetFramework>` are required. Prova auto-generates the entry point -- you do not need a `Program.cs`.

### 2. Write Tests (Standard xUnit Syntax)
```csharp
using Prova;

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
# Run as a self-executing console app (fastest)
dotnet run

# Run via the Microsoft Testing Platform (supports --coverage, --report-trx, etc.)
dotnet test
```

---

## Microsoft Testing Platform (MTP) CLI

Prova natively supports MTP CLI arguments. No adapters or bridges needed.

```bash
# List all discovered tests (0ms discovery from static registry)
dotnet run -- --list-tests

# Filter tests by name
dotnet run -- --treenode-filter="*Calculator*"

# Generate TRX report
dotnet test --report-trx

# Collect code coverage
dotnet test --coverage

# Crash dump on failure
dotnet test --crashdump

# Hang dump with timeout
dotnet test --hangdump --hangdump-timeout 30000
```

---

## Integration Packages

| Package | Description | NuGet |
|---------|------------|-------|
| [`Prova`](https://www.nuget.org/packages/Prova) | Core framework | [![NuGet](https://img.shields.io/nuget/v/Prova.svg)](https://www.nuget.org/packages/Prova) |
| [`Prova.AspNetCore`](https://www.nuget.org/packages/Prova.AspNetCore) | ASP.NET Core integration | [![NuGet](https://img.shields.io/nuget/v/Prova.AspNetCore.svg)](https://www.nuget.org/packages/Prova.AspNetCore) |
| [`Prova.Playwright`](https://www.nuget.org/packages/Prova.Playwright) | Playwright browser testing | [![NuGet](https://img.shields.io/nuget/v/Prova.Playwright.svg)](https://www.nuget.org/packages/Prova.Playwright) |
| [`Prova.Testcontainers`](https://www.nuget.org/packages/Prova.Testcontainers) | Docker container testing | [![NuGet](https://img.shields.io/nuget/v/Prova.Testcontainers.svg)](https://www.nuget.org/packages/Prova.Testcontainers) |
| [`Prova.FsCheck`](https://www.nuget.org/packages/Prova.FsCheck) | Property-based testing (FsCheck) | [![NuGet](https://img.shields.io/nuget/v/Prova.FsCheck.svg)](https://www.nuget.org/packages/Prova.FsCheck) |

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
