# Flow Control Attributes

Prova provides a set of attributes to control the execution flow of your tests, allowing you to handle flaky tests, enforce timeouts, repeat tests, and set specific cultures.

## [Retry]

The `[Retry(count)]` attribute allows you to automatically retry a failing test a specified number of times. This is useful for flaky tests that fail intermittently due to external factors.

```csharp
[Fact]
[Retry(3)] // Retry up to 3 times
public void FlakyTest()
{
    // ...
}
```

## [Timeout]

The `[Timeout(milliseconds)]` attribute enforces a maximum execution time for a test. If the test takes longer than the specified time, it will fail with a `TimeoutException`.

- **Scope**: Can be applied to Methods, Classes, or the Assembly.
- **Priority**: Method > Class > Assembly.

```csharp
[Fact]
[Timeout(500)] // Fail if takes longer than 500ms
public async Task PerformanceSensitiveTest()
{
    await ProcessingStep();
}
```

## [Repeat]

The `[Repeat(count)]` attribute runs the same test multiple times. This is useful for verify stability or load testing small units.

```csharp
[Fact]
[Repeat(10)] // Run this test 10 times
public void StabilityTest()
{
    // ...
}
```

## [Culture]

The `[Culture("name")]` attribute sets `CultureInfo.CurrentCulture` and `CultureInfo.CurrentUICulture` for the duration of the test.

```csharp
[Fact]
[Culture("fr-FR")]
public void FrenchLocaleTest()
{
    var msg = ErrorMessages.ValidationFailed;
    // Verify localized message
}
```

## [DoNotParallelize] & Resource Constraints

For controlling concurrency, see [Sequential & Parallel Execution](sequential-parallel.md) and [Resource Constraints](resource-constraints.md).
