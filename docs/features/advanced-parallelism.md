# Advanced Parallelism

Prova provides granular control over test concurrency using Limiters and Groups.

## Parallel Limiters

Use `[ParallelLimiter]` to restrict the number of concurrent tests that share a specific resource.

```csharp
public class DatabaseTests
{
    // Only 5 tests with key "DB" can run at the same time.
    [Fact]
    [ParallelLimiter("DB", 5)]
    public async Task HeavyQuery1() 
    {
        await Task.Delay(1000);
    }
}
```

This acts like a specialized Semaphore, ensuring you don't overwhelm shared resources (databases, APIs, file handles).

## Parallel Groups

Use `[ParallelGroup]` to logically group tests. Currently, this serves as metadata for reporting and future scheduling optimizations.

```csharp
[Fact]
[ParallelGroup("Integration")]
public void Test1() { }
```

## NotInParallel

The `[NotInParallel("key")]` attribute is a shorthand for `[ParallelLimiter("key", 1)]`. It enforces strict serial execution for the given resource.
