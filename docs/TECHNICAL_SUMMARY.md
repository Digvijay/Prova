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
    var tasks = new List<Task>();
    // Bounded Schedular implemented via SemaphoreSlim(maxParallel)
    using var semaphore = new SemaphoreSlim(maxParallel);

    foreach(var test in tests)
    {
        await semaphore.WaitAsync();
        tasks.Add(Task.Run(async () => {
             try {
                 // Output is bubbled up from the test logic
                 string? output = await test.ExecuteDelegate();
                 reporter.OnSuccess(test, output);
             } finally {
                 semaphore.Release();
             }
        }));
    }

    await Task.WhenAll(tasks);
}
```
This maximizes thread pool utilization while preventing starvation via the `[Parallel(max: n)]` setting.

### 4. Zero-Allocation Testing Goals

We minimize heap allocations in the runner loop:
- **Reuse**: The `ConsoleReporter` uses `Interlocked` counters rather than locking heavy objects.
- **Structs**: Internal models used during execution are kept lean.
- **No Boxing**: `Assert` methods use generic constraints (`T`) to avoid boxing value types where possible.

### 5. "Magic" Features via Compiler

Because we control the compilation:
- **`[Focus]`**: We filter the test execution set at the entry point. When `[Focus]` is active, only the targeted tests are added to the execution graph, ensuring zero distractions and immediate feedback.
- **Documentation**: We read the `SyntaxTree` XML trivia (`/// <summary>`) at compile-time and bake it directly into the runner. No XML parsing or reflection occurs at runtime.

### 6. Concurrency Boundedness
Prova implements a **Bounded Scheduler** via `SemaphoreSlim` in both the Console Runner and the MTP Adapter.
- **Default**: `Environment.ProcessorCount`.
- **Override**: Use `[Parallel(max: n)]` at the class level to specify a custom limit. Prova takes the minimum `max` found across all tests to ensure the most restrictive environment is honored.

### 7. Output Capture
Capturing stdout in parallel/async tests is difficult. `Console.SetOut` is not thread-safe.
Prova solves this by injecting a per-test `ITestOutputHelper`.
- The generator creates a new `TestOutputHelper` instance for each test.
- The `ExecuteDelegate` signature is modified from `Func<Task>` to `Func<Task<string?>>`.
- Logs are strictly isolated and returned to the runner *after* execution, ensuring no cross-talk between thread logs.

### 8. AOT Dependency Injection (Factory Pattern)
To avoid runtime reflection (scanning constructors), Prova uses an **Explicit Factory Pattern**:
- **Setup**: Users mark static factory methods with `[TestDependency]`.
- **Index**: The analyzer builds a `ReturnType -> MethodCall` map at compile time.
- **Wiring**: When generating test instantiation (`new MyTest(dep)`), the emitter resolves dependencies from this map directly in source code.
- **Result**: Zero DI container overhead. Zero runtime resolution. 100% Native AOT safe.

### 9. Governance: Prophetic Allocation Monitoring
To enforce `[MaxAlloc(n)]`, Prova does **not** use a profiler API or complex instrumentation.
- **Mechanism**: The generator simply wraps the test call in a `try/finally` block.
- **Measurement**: `GC.GetAllocatedBytesForCurrentThread()` is called before and after the test delegate.
- **Cost**: If `[MaxAlloc]` is missing, **0 instructions** are emitted. The overhead is strictly pay-for-play.
- **Safety**: Because this happens inside the `Task.Run` delegate, it correctly attributes allocations to the specific test thread (even in parallel execution).

### Summary for Infrastructure Teams

Prova is designed to be embedded in high-performance CI/CD pipelines where:
- **Startup time matters** (Micro-benchmarking, rapid inner loop).
- **Binary size matters** (WASM, Mobile, IoT scenarios).
- **Stability matters** (Compiler-verified test definitions).

It is not just a test framework; it is a **Static Analysis Tool** that happens to run tests.
