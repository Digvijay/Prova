# Custom Executors

Custom Executors allow you to control how tests and lifecycle hooks are invoked. This is useful for scenarios like:
- Running tests in a specific thread (e.g., STA for UI tests).
- Implementing custom retry or error handling logic.
- Integrating with external execution environments.

## Interfaces

Prova provides two interfaces for custom execution:

### ITestExecutor
Controls the execution of a test method.

```csharp
public interface ITestExecutor
{
    Task<string?> ExecuteAsync(Func<Task<string?>> testAction, ProvaTest testInfo);
}
```

### IHookExecutor
Controls the execution of lifecycle hooks (`[Before]`, `[After]`, etc.).

```csharp
public interface IHookExecutor
{
    Task ExecuteAsync(Func<Task> hookAction, string hookName);
}
```

## The [Executor] Attribute

Use the `[Executor]` attribute to specify which executor to use. It can be applied at the method, class, or assembly level.

```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
public class ExecutorAttribute : Attribute
{
    public Type ExecutorType { get; }
    public ExecutorAttribute(Type executorType) => ExecutorType = executorType;
}
```

## Example: STA Executor

If you need to run tests on a Single-Threaded Apartment (STA) thread, you can implement an `StaExecutor`:

```csharp
public class StaExecutor : ITestExecutor
{
    public async Task<string?> ExecuteAsync(Func<Task<string?>> testAction, ProvaTest testInfo)
    {
        var tcs = new TaskCompletionSource<string?>();
        var thread = new Thread(async () =>
        {
            try { tcs.SetResult(await testAction()); }
            catch (Exception ex) { tcs.SetException(ex); }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return await tcs.Task;
    }
}
```

Usage:

```csharp
public class MyUiTests
{
    [Fact]
    [Executor(typeof(StaExecutor))]
    public void Test_In_STA()
    {
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
    }
}
```

## AOT Compatibility & Resolution

Custom Executors are **100% AOT compatible**. Prova's source generator automatically detects all used executors and:
1.  Generates **static type references** using `typeof()`.
2.  Generates **automatic DI registrations** in the `RunAllAsync` method.

### Resolution Logic
When a test or hook starts:
1.  Prova tries to resolve the executor from the `Services` container (the `ProvaServiceCollection`).
2.  Since executors are auto-registered, they are resolved without any runtime reflection or `Activator.CreateInstance` calls.

You can still manually register executors if you need to provide custom constructor arguments:

```csharp
[ConfigureServices]
public static void Setup(ProvaServiceCollection services)
{
    // Override auto-registration with a custom instance or factory
    services.AddSingleton<MyExecutor>(new MyExecutor("custom setting"));
}
```
