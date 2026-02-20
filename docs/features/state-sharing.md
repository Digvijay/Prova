# State Sharing (Global State)

In complex test suites, you often need to share state across different phases of the test run—for example, initializing a database connection string in a global hook and accessing it within your tests.

Prova provides the `TestContext.GlobalState` property (backed by a thread-safe `StateBag`) to facilitate this.

## How it Works

`TestContext.GlobalState` is a static singleton available throughout the entire lifecycle of the test runner. You can set values during initialization (e.g., in `[BeforeAssembly]`) and retrieve them in your tests.

## Usage

### 1. Setting State (e.g., in a Hook)

```csharp
using Prova;

public class GlobalSetup
{
    [BeforeAssembly]
    public static void Initialize()
    {
        // Calculate or retrieve expensive data once
        var connectionString = "Server=myServerAddress;Database=myDataBase;";
        
        // Store it globally
        TestContext.GlobalState.Set("ConnectionString", connectionString);
        TestContext.GlobalState.Set("StartTime", DateTime.Now);
    }
}
```

### 2. Accessing State (in a Test)

```csharp
using Prova;

public class DatabaseTests
{
    [Fact]
    public void VerifyConnection()
    {
        // Retrieve the data safely
        var connStr = TestContext.GlobalState.Get<string>("ConnectionString");
        
        Assert.StartsWith("Server=", connStr);
    }

    [Fact]
    public void CheckTime()
    {
        // TryGet pattern avoids exceptions if key is missing
        if (TestContext.GlobalState.TryGet<DateTime>("StartTime", out var time))
        {
            Console.WriteLine($"Test run started at: {time}");
        }
    }
}
```

## API Reference

The `StateBag` provides the following thread-safe methods:

- `void Set<T>(string key, T value)`: Adds or updates a key.
- `T Get<T>(string key)`: Retrieves a value. Throws `KeyNotFoundException` if missing.
- `bool TryGet<T>(string key, out T? value)`: Safely attempts to retrieve a value.
- `object? this[string key]`: Indexer for direct access (returns `object?`).

## Best Practices

1.  **Immutability**: Ideally, store immutable objects (strings, configuration records) in the Global State to avoid side effects from parallel tests modifying the same object.
2.  **Thread Safety**: `StateBag` itself is thread-safe, but the *objects* you store in it might not be. If multiple tests modify a stored object simultaneously, you must ensure that object is thread-safe.
3.  **Cleanup**: You can use `[AfterAssembly]` to clean up resources stored in the Global State if necessary.
