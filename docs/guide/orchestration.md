# Orchestration

Managing complex test infrastructure—like databases, message brokers, and external APIs—is a common challenge in integration testing. Prova provides several tools to help orchestrate these dependencies.

## Lifecycle Hooks

Use lifecycle hooks to manage the setup and teardown of shared infrastructure.

- **`[BeforeAll]` / `[AfterAll]`**: Perfect for global infrastructure that should persist across all tests in a class or assembly.
- **`[BeforeEach]` / `[AfterEach]`**: Use these for state that must be reset for every single test.

```csharp
public class DatabaseTests : IAsyncLifetime
{
    private MyDb _db;

    public async Task InitializeAsync()
    {
        _db = await Container.StartSqlAsync();
    }

    public async Task DisposeAsync()
    {
        await _db.StopAsync();
    }
}
```

## Service Discovery

Prova integrates tightly with dependency injection, allowing you to easily resolve services needed for your tests.

- **`[ClassFactory]`**: Use a custom factory to instantiate your test classes with complex dependencies.
- **`[ClassDataSource]`**: Inject data directly into your test class constructors from DI.

## Integration Tools

Prova ecosystem includes first-class support for industry-standard orchestration tools.

- **Testcontainers**: Use the `Prova.Testcontainers` package to spin up transient Docker containers for databases and other services.
- **.NET Aspire**: Leverage the `Aspire.Hosting.Testing` library to test multi-service applications as a single unit.
- **Playwright**: Orchestrate browser instances for full end-to-end UI testing.

## Test Isolation

To prevent flaky tests, ensure that each test run is isolated:
1. **Prefer Transient State**: Use unique database schemas or ports for each test run where possible.
2. **Clean Teardown**: Always use `IAsyncLifetime` or `[AfterAll]` to shut down resources, even if a test fails.
3. **Handle Timeouts**: Use the `[Timeout]` attribute to ensure a hung orchestration step doesn't block your entire CI pipeline.
