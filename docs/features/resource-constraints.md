# Parallel Resource Constraints 🚦

Prova runs tests in parallel by default to maximize performance. However, some tests require exclusive access to shared resources (like a database, file system path, or network port). Prova provides the `[NotInParallel]` attribute to handle these scenarios without disabling parallelism globally.

## The Problem

If two tests try to write to the same file or database table simultaneously, they might fail or produce inconsistent results.

```csharp
[Fact]
public async Task WriteToDatabase()
{
    // Race condition if run in parallel with another DB test!
    await _db.Insert("user1"); 
}
```

## The Solution: `[NotInParallel]`

Use `[NotInParallel("Key")]` to declare that a test uses a specific resource. Prova ensures that **no two tests with the same resource key run at the same time**.

```csharp
using Prova;

public class DatabaseTests
{
    [Fact]
    [NotInParallel("Database")] // Claims the "Database" lock
    public async Task Test1()
    {
        await _db.Insert("user1");
    }

    [Fact]
    [NotInParallel("Database")] // Waits for "Database" lock
    public async Task Test2()
    {
        await _db.Insert("user2");
    }
}
```

In this example:
1. `Test1` starts and acquires the "Database" lock.
2. `Test2` attempts to start, sees "Database" is locked, and waits.
3. `Test1` finishes and releases the lock.
4. `Test2` starts.

### Non-Conflicting Parallelism

Tests with *different* resource keys (or no keys) continue to run in parallel!

```csharp
[Fact]
[NotInParallel("Database")]
public void DB_Test() { ... }

[Fact]
[NotInParallel("FileSystem")]
public void File_Test() { ... } // Runs in parallel with DB_Test!

[Fact]
public void CPU_Test() { ... } // Runs in parallel with everything!
```

## Multiple Resources

You can declare multiple resources for a single test. The test will wait until *all* locks are acquired.

```csharp
[Fact]
[NotInParallel("Database", "FileSystem")]
public void IntegrationTest()
{
    // Has exclusive access to BOTH Database and FileSystem
}
```

## Class-Level Constraints

You can apply the attribute to a class to enforce the constraint on *all* tests within that class.

```csharp
[NotInParallel("Database")]
public class UserRepoTests
{
    [Fact]
    public void AddUser() { ... } // Implicitly claims "Database"

    [Fact]
    public void DeleteUser() { ... } // Implicitly claims "Database"
}
```

::: tip
Using `[NotInParallel]` is much more performant than disabling parallelism entirely with `[DoNotParallelize]`, because it allows non-conflicting tests to keep running fast.
:::
