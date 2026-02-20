# Global Policies (Assembly Defaults)

Prova allows you to define global execution policies at the assembly level. These defaults apply to all tests in the project unless overridden at the Class or Method level.

## Supported Policies

You can set assembly-level defaults for:

- **Retry**: How many times to retry a failed test.
- **Timeout**: The default execution timeout for tests.
- **Parallelism**: The maximum number of concurrent tests or serial execution.

## Usage

Define policies in any file (usually `AssemblyInfo.cs` or `GlobalUsings.cs`) using `[assembly: ...]` syntax.

```csharp
using Prova;

// All tests will retry twice upon failure
[assembly: Retry(2)]

// All tests have a default timeout of 10 seconds
[assembly: Timeout(10000)]

// Limit the entire assembly to 4 concurrent tests
[assembly: Parallel(4)]
```

## Resolution Hierarchy

Prova resolves these settings using the following priority (first match wins):

1. **Method Level**: Attribute applied directly to the test method.
2. **Class Level**: Attribute applied to the test class.
3. **Assembly Level**: Attribute applied to the assembly.
4. **Environment Default**: Prova's internal defaults (e.g., unlimited parallelism).

### Example: Overriding Defaults

```csharp
[assembly: Retry(3)] // Global default

public class MyTests
{
    [Fact]
    [Retry(0)] // Overrides global default for this test
    public void NonRetriableTest() { ... }

    [Fact]
    public void RetriableTest() { ... } // Uses global default (3 retries)
}
```

## Parallelism vs Sequential

If your assembly is marked as `[assembly: Sequential]`, all tests will run serially by default. You can still parallelize specific classes or methods by adding `[Parallel]` to them.

```csharp
[assembly: Sequential]

public class ParallelClass
{
    [Fact]
    [Parallel] // This test can run in parallel with others in this class
    public void Test1() { ... }
}
```
