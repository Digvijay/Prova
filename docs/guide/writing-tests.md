# Writing Tests

Prova simplifies the test authoring experience by adhering to standard conventions while optimizing for AOT.

## Test Discovery

Prova does not use reflection at runtime. Instead, it uses **Source Generators** to discover tests during compilation.
- **Rules**:
    - Classes must be `public` (or `internal` if `InternalVisibleTo` is set).
    - Methods must be annotated with `[Fact]` or `[Theory]`.
    - Methods can be `void` or `Task` / `ValueTask`.

## Attributes

### `[Fact]`
Defines a test that has no input arguments.

```csharp
[Fact]
public void BasicTest()
{
    Assert.True(true);
}
```

### `[Theory]` & `[InlineData]`
Defines a data-driven test.

```csharp
[Theory]
[InlineData(1, 1, 2)]
[InlineData(10, 5, 15)]
public void Add_Works(int a, int b, int expected)
{
    Assert.Equal(expected, a + b);
}
```

## Assertions

Prova bundles a fluent and performant assertion library `Prova.Assertions`.

```csharp
int value = 42;

// Standard
Assert.Equal(42, value);

// Fluent
Assert.That(value).IsEqualTo(42);
```

See the **[Assertions Reference](../api/assertions.md)** for the full list.

## Lifecycle

Use `IAsyncLifetime` for setup and teardown logic.

```csharp
public class DatabaseTests : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await Database.Connect();
    }

    [Fact]
    public void CanQuery() { ... }

    public async Task DisposeAsync()
    {
        await Database.Disconnect();
    }
}
```
