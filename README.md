# Prova 🇸🇪

![Build Status](https://img.shields.io/github/actions/workflow/status/Digvijay/Prova/ci.yml?branch=master)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![NuGet](https://img.shields.io/nuget/v/Prova.svg)

> [!NOTE]
> **Research Project**: Prova is a reference implementation for Native AOT testing patterns. It is community-driven and not an official Microsoft product.

**Prova** is a next-generation testing framework for .NET, built for **Speed**, **Native AOT**, and **Developer Experience**. 

It eliminates reflection overhead by leveraging Roslyn Source Generators to discover and run tests at compile time, enabling true zero-overhead startup and seamless AOT and trimming support.

## ✨ Features

- **⚡ Zero Reflection / Native AOT**: Fully compatible with `PublishAot`. No runtime discovery cost.
- **🏃 True Parallelism**: Test Classes run concurrently by default (`Task.WhenAll`), maximizing CPU usage.
- **🧙‍♂️ Magic Documentation**: Your `/// <summary>` test comments are automatically extracted and displayed in the runner output.
- **🎯 Focus Mode**: Use `[Focus]` to run *only* the tests you're debugging (compile-time filtering).
- **🛡️ Flake Free**: Use `[Retry(3)]` to automatically retry flaky tests.
- **🔗 Smart Verify (Nordic Suite)**: Automated integration with [Skugga](https://github.com/Digvijay/Skugga). No more manual `.VerifyAll()` calls. [Learn more](docs/SKUGGA_INTEGRATION.md).
- **📦 xUnit Parity**:
  - `[Fact]`, `[Theory]`, `[InlineData]`, `[MemberData]`
  - `IClassFixture<T>` (Singleton Fixtures)
  - `IAsyncLifetime` (Async Setup/Teardown)
  - `[Trait]` categories & filtering
  - Full `Assert` suite (`Equal`, `Throws`, `Contains`, `Single`, etc.)

## 🚀 Quick Start

1. **Install Prova**:
   ```bash
   dotnet add package Prova
   ```



2. **Write a Test**:
   ```csharp
   using Prova;

   public class CalculatorTests
   {
       [Fact]
       public void Add_ReturnsSum()
       {
           Assert.Equal(4, 2 + 2);
       }

       /// <summary>
       /// Simple division test ➗
       /// </summary>
       [Theory]
       [InlineData(10, 2, 5)]
       public void Divide_ReturnsQuotient(int a, int b, int expected)
       {
           Assert.Equal(expected, a / b);
       }
   }
   ```

3. **Run**:
   
   **Option A: Direct Execution (Fastest / Recommended)**
   ```bash
   dotnet run
   ```

   **Option B: `dotnet test` (CI/CD)**
   To use `dotnet test`, add a `global.json` to your solution root:
   ```json
   {
       "test": {
           "runner": "Microsoft.Testing.Platform"
       }
   }
   ```
   Then run:
   ```bash
   dotnet test --project MyTestProject.csproj
   ```

4. **Microsoft Testing Platform (MTP)**:
   Prova fully supports the new Microsoft Testing Platform. Check out the [MTP Sample](samples/Prova.MtpSample) for a complete example.
   
   **Run with TRX Reporting:**
   ```bash
   dotnet run --project samples/Prova.MtpSample -- --report-trx
   ```

## 🛠️ Developer Experience

### Focus Mode
Working on a specific test? Don't run the whole suite. Just add `[Focus]`.

```csharp
[Fact]
[Focus] // <--- Only this test will run!
public void MyNewFeature() { ... }
```

> **Why `[Focus]`?**
> - **Zero Overhead**: Unlike runtime filtering, `Prova` *only generates code* for focused tests. Skipped tests aren't even allocated.
> - **Convenience**: No more complex CLI args like `dotnet test --filter "FullyQualifiedName~MyTest"`. Just tag and run.

### Retry Flaky Tests
Have a test that fails sporadically due to network blips?

```csharp
[Fact]
[Retry(3)] // <--- Retries up to 3 times before failing
public void NetworkTest() { ... }
```

## 🤝 Contributing

We love contributions! Please read our [CONTRIBUTING.md](CONTRIBUTING.md) to get started.

## 📄 License

Prova is fully open source and licensed under [MIT](LICENSE).
