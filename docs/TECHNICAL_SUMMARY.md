# Prova: Technical Summary (Deep Dive)

**Target Audience:** Architects, Framework Authors, Performance Engineers.

> [!CAUTION]
> **Project Scope**: Prova is a standalone **reference implementation** and an **experimental research project**. It is intended to showcase the potential of Roslyn-based testing architectures. It is not associated with Microsoft Corporation but provides a **Hybrid MTP Adapter** for compatibility with the modern .NET testing ecosystem.

## Core Philosophy: "Compile-Time is the new Runtime"

Prova is a fundamental rethink of .NET testing infrastructure, moving the heavy lifting of test discovery and orchestration from **Runtime Reflection** to **Roslyn Source Generators**.

### 1. The Source Generator Architecture

Instead of `Assembly.GetTypes().SelectMany(t => t.GetMethods())...` at runtime, Prova employs an `IIncrementalGenerator` pipeline:

```csharp
// High-Level Pipeline
var testMethods = context.SyntaxProvider
    .CreateSyntaxProvider(
        predicate: (node, _) => IsCandidate(node),
        transform: (ctx, _) => ModelFactory.Create(ctx) // Expensive semantic analysis
    )
    .Where(model => model != null);

context.RegisterSourceOutput(testMethods, (spc, methods) => 
{
    // Generates a Single Static Entry Point
    SourceEmitter.Emit(spc, methods);
});
```

**Key Advantages:**
- **Zero-Cost Abstractions**: Attributes like `[Fact]` and `[Trait]` are peeled away at compile time. The resulting code is just raw `Task` invocation.
- **Tree Shaking / Trimming**: Since test calls are static references (`MethodA()`), the IL linker (ILLink) knows exactly what is used. Unused code is aggressively trimmed.
- **Diagnostics**: Invalid test signatures (e.g., `void` instead of `Task` for async) are caught as **Compiler Errors** (or Analyzer warnings), not runtime failures.

### 2. Native AOT Compatibility

Traditional frameworks (xUnit, NUnit, MSTest) rely on:
- `System.Reflection.Emit` (Dynamic Proxies)
- `RuntimeHelpers.RunClassConstructor`
- Extensive use of `MakeGenericType`

This makes AOT challenging (requiring massive `rd.xml` directives).

**Prova is AOT-Native by default.**
- No dynamic code generation.
- No `Type.MakeGenericType` at runtime (handled by generating generic instantiations at compile time).
- No Reflection scanning.

### 3. Concurrency Model: `Task.WhenAll`

Prova treats the test suite as a massive async graph.

**Generated Code Structure:**
```csharp
public static async Task RunAllAsync()
{
    var classTasks = new List<Task>();
    
    // Class A
    classTasks.Add(Task.Run(async () => {
         // Isolation: Fixtures are currently created per-test for AOT safety
         var fixture = new DatabaseFixture();
         await fixture.InitializeAsync();
         
         // Test 1
         try { await new ClassA(fixture).Test1(); } ...
         
         // Test 2
         try { await new ClassA(fixture).Test2(); } ...
    }));

    // Class B (Runs in Parallel with A)
    classTasks.Add(Task.Run(async () => { ... }));

    await Task.WhenAll(classTasks);
}
```
This maximizes thread pool utilization (`ThreadPool.QueueUserWorkItem`) without manual thread management.

### 4. Zero-Allocation Testing Goals

We minimize heap allocations in the runner loop:
- **Reuse**: The `ConsoleReporter` uses `Interlocked` counters rather than locking heavy objects.
- **Structs**: Internal models used during execution are kept lean.
- **No Boxing**: `Assert` methods use generic constraints (`T`) to avoid boxing value types where possible.

### 5. "Magic" Features via Compiler

Because we control the compilation:
- **`[Focus]`**: We filter the *generation* of the `classTasks` list. Focused tests literally mean *only those lines of code exist* in the runner path.
- **Documentation**: We read the `SyntaxTree` XML trivia (`/// <summary>`) and bake it into the string table of the runner. No XML parsing at runtime.

### Summary for Infrastructure Teams

Prova is designed to be embedded in high-performance CI/CD pipelines where:
- **Startup time matters** (Micro-benchmarking, rapid inner loop).
- **Binary size matters** (WASM, Mobile, IoT scenarios).
- **Stability matters** (Compiler-verified test definitions).

It is not just a test framework; it is a **Static Analysis Tool** that happens to run tests.
