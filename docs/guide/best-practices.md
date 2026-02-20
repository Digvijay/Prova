# Best Practices

To get the most out of Prova, follow these best practices for naming, organization, and assertions.

## Naming Conventions

Consistent naming makes tests easier to read and results easier to analyze.

### Test Methods
Use a descriptive pattern like `MethodName_Scenario_ExpectedResult`.

- **Good**: `Withdraw_SufficientFunds_UpdatesBalance`
- **Avoid**: `Test1`, `WithdrawTest`

### Display Names
Use the `[DisplayName]` attribute for human-readable reports, especially for parameter-driven tests.

```csharp
[DisplayName("Withdraw ${0} from account with ${1}")]
[InlineData(100, 500)]
public void Withdraw_Funds(int amount, int balance) { ... }
```

## Organization

### File Structure
Group tests by the feature or component they verify.

```text
/tests
  /Identity
    LoginTests.cs
    RegisterTests.cs
  /Billing
    InvoiceTests.cs
```

### Class Structure
Use classes to group related tests and manage shared state or lifecycle hooks.

```csharp
public class OrderTests
{
    [BeforeEach]
    public void Setup() { ... }

    [Fact]
    public void PlaceOrder_Valid() { ... }
}
```

## Assertions

### Choose the Right Assert
Use specific assertions for better error messages.

- **Prefer**: `Assert.Empty(list)` over `Assert.Equal(0, list.Count)`
- **Prefer**: `Assert.Null(obj)` over `Assert.Equal(null, obj)`

### Custom Messages
Provide context when an assertion fails, especially in complex loops or data-driven tests.

```csharp
Assert.NotNull(user, "User should be created after registration");
```

### Avoid Multiple Assertions
Aim for one logical assertion per test. If a test fails, you should know exactly why without debugging which assertion failed.
