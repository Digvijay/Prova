# ⚡ Parallel & Sequential Execution

Prova allows you to control how tests are executed using concurrency attributes at the Class or Assembly level.

## 🚀 Parallel Execution

By default, Prova runs tests in parallel, capped at the number of processors on your machine. You can explicitly control this behavior.

### At the Class Level
Use `[Parallel(n)]` to limit the number of concurrent tests within a specific class.

```csharp
[Parallel(2)] // Max 2 tests in this class will run at once
public class MyTests { ... }
```

Use `[Parallel]` (without arguments) to indicate that the class has no specific concurrency limits and should use the global runner default.

### At the Assembly Level
Apply `[Parallel(n)]` to your assembly (usually in `AssemblyInfo.cs` or any file) to set the global default concurrency for the entire test runner.

```csharp
[assembly: Parallel(16)]
```

---

## 🧵 Sequential Execution

If your tests are sensitive to shared state or timing, you can force them to run one at a time.

### At the Class Level
Use `[Sequential]` to ensure all tests in the class run serially.

```csharp
[Sequential]
public class IntegrationTests
{
    [Fact]
    public void TestA() { ... }

    [Fact]
    public void TestB() { ... } // Will wait for TestA to finish
}
```

### At the Assembly Level
Force the entire test suite to run sequentially by applying it to the assembly.

```csharp
[assembly: Sequential]
```

---

## 🔒 Resource Constraints

For even finer control, use `[NotInParallel]` to prevent specific tests from running alongside others that use the same shared resource (e.g., a Database or File).

```csharp
public class ResourceTests
{
    [Fact]
    [NotInParallel("Database")]
    public void Test1() { ... }

    [Fact]
    [NotInParallel("Database")]
    public void Test2() { ... } // Will not run at the same time as Test1
}
```

> [!TIP]
> Use `[DoNotParallelize]` to mark a test that must run in complete isolation from ALY other tests.
