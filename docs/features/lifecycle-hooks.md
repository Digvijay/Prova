# Lifecycle Hooks 🔄

Lifecycle hooks allow you to execute code at specific points in the test execution lifecycle. Prova supports three scopes: **Test**, **Class**, and **Assembly**.

## Hook Scopes

| Scope | Attribute | Shorthand | Requirements | Description |
|-------|-----------|-----------|--------------|-------------|
| **Test** | `[Before]`, `[After]` | | Instance method | Runs before/after every test method in the class. |
| **Class** | `[Before(HookScope.Class)]`, `[After(HookScope.Class)]` | `[BeforeClass]`, `[AfterClass]` | `static` method | Runs once before the first test and after the last test in the class. |
| **Assembly** | `[Before(HookScope.Assembly)]`, `[After(HookScope.Assembly)]` | `[BeforeAssembly]`, `[AfterAssembly]` | `static` method | Runs once at the very start and very end of the entire test run. |
| **Global** | `[BeforeEvery]`, `[AfterEvery]` | | `static` method | Runs before/after **every** test or class across the entire assembly. |

## Usage Examples

### Test Level (Default)

Test-level hooks are ideal for resetting state between individual tests.

```csharp
public class MyTests
{
    [Before]
    public void Setup() 
    {
        // Reset database, clear logs, etc.
    }

    [After]
    public void Teardown() 
    {
        // Cleanup resources
    }

    [Fact]
    public void Test1() { }
}
```

### Class Level

Class hooks are useful for heavy initialization that can be shared across all tests in a single class. **These must be marked as `static`.**

```csharp
public class MyTests
{
    [BeforeClass]
    public static void GlobalSetup() 
    {
        // Start a shared container or service
    }

    [AfterClass]
    public static void GlobalTeardown() 
    {
        // Stop the shared container
    }
}
```

### Assembly Level

Assembly hooks run globally. They can be placed in any class within your test assembly. **These must be marked as `static`.**

```csharp
public static class GlobalHooks
{
    [BeforeAssembly]
    public static void Init() 
    {
        // Initialize global logging or environment
    }

    [AfterAssembly]
    public static void Cleanup() 
    {
        // Final global cleanup
    }
}
```

### Global Hooks

Global hooks run for **every** test or class in the assembly. These are defined once but applied everywhere. **These must be marked as `static` and can be `async`.**

| Scope | Attribute | Description |
|-------|-----------|-------------|
| **Test** | `[BeforeEvery(HookScope.Test)]` | Runs before **every** test method in the assembly. |
| **Class** | `[BeforeEvery(HookScope.Class)]` | Runs before **every** test class in the assembly. |

```csharp
public static class MyGlobalHooks
{
    [BeforeEvery(HookScope.Class)]
    public static async Task GlobalClassSetup() 
    {
        // Runs before EVERY class starts
    }
}
```

> [!TIP]
> Use `[BeforeEvery]` for cross-cutting concerns like global logging, performance tracking, or clearing thread-local storage.

## Async Support

Currently, `Class` and `Assembly` hooks support `Task` returning methods for asynchronous initialization.

```csharp
[Before(HookScope.Assembly)]
public static async Task InitAsync() 
{
    await Task.Delay(100); // Async initialization
}
```

> [!IMPORTANT]
> Prova's source generator automatically wires these hooks at compile-time, ensuring **zero-reflection** overhead during test execution.
