# Prova Architecture

> [!NOTE]
> Prova is an **Experimental Research Prototype** for Native AOT testing. It is a standalone project independent of Microsoft. While it is a custom runner, it supports the **Microsoft Testing Platform (MTP)** via a hybrid adapter.

## Directory Structure

```
src/
  Prova.Core/           # Runtime Library (Attributes, Assert, Interfaces)
    Abstractions/       # ITestReporter, IClassFixture, IAsyncLifetime
    Framework/          # [Fact], [Theory], Assert
    Reporters/          # ConsoleReporter
  Prova.Generators/     # The Brains 🧠 (Roslyn Source Generators)
    Analysis/           # SyntaxAnalyzer (Reads your code)
    Emission/           # SourceEmitter (Writes the runner)
    Models/             # Intermediate Representations (MethodModel, etc.)

tests/
  Prova.Core.Tests/     # The Test Suite (Prova testing itself)
  
samples/
  Prova.Demo/           # Showcase Console App
```

## The Pipeline

1.  **Code Analysis (`SyntaxAnalyzer`)**:
    - Scans for `[Fact]`, `[Theory]`.
    - Extracts Metadata: `[Retry]`, `[Focus]`, `[Trait]`, XML Docs.
    - Validates Signatures (Must be `void` or `Task`).

2.  **Model Creation**:
    - Converts Roslyn Symbols (`IMethodSymbol`) into simple Records (`TestMethodModel`).
    - Resolves dependencies (`IClassFixture`, `ITestOutputHelper`).

3.  **Code Emission (`SourceEmitter`)**:
    - Generates `Prova.Generated.TestRunnerExecutor`.
    - Injects `Main` entry point logic.
    - Handles Try/Catch, Interlocked counters, and Reporter callbacks.

## Key Components

### `ITestReporter`
Abstraction for reporting results. Currently `ConsoleReporter` is implemented, but this allows for:
- TRX Reporters (CI)
- JSON Reporters
- GitHub Actions Reporters

### `ITestOutputHelper`
Captures output *per test*.
- Prova generates a local `TestOutputHelper` for each test.
- Output is passed to the reporter only upon test completion (to prevent interleaved console garbage).

### `IClassFixture<T>`
Simple Dependency Injection.
- **Isolated Scope**: Currently created per-test for maximum AOT safety and parallel isolation.
- **Injected**: Passed to the Test Class constructor.
- **Lifecycle**: `InitializeAsync` -> Test Runs -> `DisposeAsync`.
