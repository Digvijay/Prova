# Test Properties & Filtering

Prova allows you to attach arbitrary key-value metadata to your tests using the `[Property]` and `[Trait]` attributes. These can be used for reporting, organization, and filtering test execution via the CLI.

## Attributes

### `[Property]`
The standard way to add metadata in Prova.

```csharp
[Fact]
[Property("Category", "Integration")]
public void MyTest() { ... }
```

### `[Trait]`
An alias for `[Property]`, provided for compatibility with xUnit.

```csharp
[Fact]
[Trait("Category", "Unit")]
public void MyUnit() { ... }
```

## Scoping & Overrides

Properties can be applied at the **Class** or **Method** level.

- **Class Level**: Properties apply to all tests in the class.
- **Method Level**: Properties apply only to that specific test. If the same key exists at the class level, the method-level value **overrides** it.

```csharp
[Property("Owner", "Digvijay")]
public class MyTests
{
    [Fact]
    public void DefaultOwnerTest() { ... } // Owner = Digvijay

    [Fact]
    [Property("Owner", "Architect")]
    public void OverriddenOwnerTest() { ... } // Owner = Architect
}
```

## CLI Filtering

You can filter which tests to run using the `--filter` argument in the CLI.

### Exact Key=Value Match
Run only tests that have a specific property value.

```bash
dotnet run -- --filter Category=Integration
```

### Multiple Filters (AND)
You can provide multiple filters; a test must satisfy **all** of them to run.

```bash
dotnet run -- --filter Category=Integration --filter Priority=High
```

### Key or Value Presence
If you provide a string without an `=`, Prova will run tests that have that string as either a **key** or a **value**.

```bash
dotnet run -- --filter Smoke
```

### Keyword Search
You can also provide a simple string that doesn't start with `--` to filter by **DisplayName** or any **Property Value**.

```bash
dotnet run -- integration
```

## Accessing Properties in Code
The `Properties` dictionary is available on the `ProvaTest` object within the runner or reporters.

```csharp
public void OnTestStarting(string name, string? description)
{
    // Access properties if needed
}
```
