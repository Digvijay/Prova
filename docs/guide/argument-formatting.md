# Argument Display Formatters

Prova allows you to completely customize how arguments are displayed in test names using the `[ArgumentDisplayFormatter]` attribute. This is useful for complex types or when you want a specific representation (e.g., Hexadecimal).

## Creating a Formatter

Implement the `IArgumentFormatter` interface.

```csharp
using Prova;

public class HexFormatter : IArgumentFormatter
{
    public string Format(object? value)
    {
        if (value is int i) return $"0x{i:X}";
        return value?.ToString() ?? "null";
    }
}
```

## Applying a Formatter

Apply the `[ArgumentDisplayFormatter(typeof(YourFormatter))]` attribute to test parameters.

```csharp
[Fact]
[DisplayName("Checking value {0}")]
public void HexTest([Matrix(10, 255)] [ArgumentDisplayFormatter(typeof(HexFormatter))] int value)
{
    // Display name will be: Checking value 0xA
    // Display name will be: Checking value 0xFF
}
```

## Supported Scenarios

- **Combinatorial (`[Matrix]`)**: Fully supported.
- **MemberData / ClassData**: Supported (formatter is applied to the casted value).
- **InlineData**: *Not yet supported* (values are literals).

## Best Practices

- Formatters should be lightweight and exception-safe.
- Use `[DisplayName]` with `{0}` placeholders to leverage the formatted string.
