# ⚡ Parallelism

Prova is designed for maximum performance through non-blocking, asynchronous parallelism. By default, Prova runs tests across all available CPU cores.

## 🛠️ Concurrency Control

While Prova encourages thread-safe tests, some scenarios require restricted concurrency.

### [Parallel(n)]
Limit the number of concurrent tests *within* a specific class.

```csharp
[Parallel(2)] // Only 2 tests in this class run at once
public class DatabaseTests { ... }
```

> [!NOTE]
> Unlike other frameworks, a class-level `[Parallel(1)]` in Prova does **not** stop other classes from running in parallel. It only serializes tests within that specific class.

### [Serial]
A convenient alias for `[Parallel(1)]`. Ensures that tests in the marked class run one after another.

```csharp
[Serial]
public class IntegrationTests { ... }
```

### [DoNotParallelize]
The "nuclear option" for tests that are not thread-safe at a global level (e.g., modifying static state or environment variables).

When a test marked with `[DoNotParallelize]` runs, Prova:
1. Waits for all currently running tests to complete.
2. Executes the isolated test alone.
3. Resumes parallel execution for remaining tests.

```csharp
[Fact, DoNotParallelize]
public void EnvironmentSensitiveTest() { ... }
```

## ⚙️ Global Limits
By default, Prova uses `Environment.ProcessorCount` as the global concurrency limit. This ensures your machine stays responsive while running thousands of tests.
