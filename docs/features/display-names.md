# Custom Display Names

Prova allows you to customize the display name of tests using the `[DisplayName]` attribute. This is useful for creating readable test reports, especially for data-driven or combinatorial tests.

## Basic Usage

Apply `[DisplayName]` to a test method to override the default text (which is typically `ClassName.MethodName`).

```csharp
[Fact]
[DisplayName("User Registration Flow")]
public void Test1()
{
    // ...
}
```

## Formatting Arguments

The `[DisplayName]` attribute supports standard composite formatting strings. You can use `{0}`, `{1}`, etc., to refer to the arguments passed to the test method.

This works with `[InlineData]`, `[ClassData]`, `[MemberData]`, and `[Matrix]`.

```csharp
[Theory]
[InlineData(10, "High")]
[InlineData(1, "Low")]
[DisplayName("Priority {0} is labeled '{1}'")]
public void PriorityTest(int level, string label)
{
    // ...
}
```

## Combinatorial Tests

When using `[Matrix]`, the arguments are mapped in the order they appear in the method signature.

```csharp
[Fact]
[DisplayName("User: {0}, Active: {1}")]
public void UserTest([Matrix("Alice", "Bob")] string user, [Matrix(true, false)] bool active)
{
    // ...
}
```

## Emojis in Display Names

Thanks to Prova's auto-generated entry point (introduced in v0.5.0), the console's `OutputEncoding` and `InputEncoding` are forced to `UTF-8`. This means you can safely use emojis in `[DisplayName]` or string arguments, and they will render perfectly across Windows Terminal, PowerShell, and bash without mangling!

```csharp
[Fact]
[DisplayName("🔌 Database Initialization Test")]
public void Setup_Database()
{
}
```

## Best Practices

- Use display names to describe the **scenario** being tested rather than the technical method name.
- Include parameter values in the display name to distinguish between data-driven test cases easily.
