# Property-Based Testing with FsCheck

Prova integrates seamlessly with [FsCheck](https://fscheck.github.io/FsCheck/) to enable robust Property-Based Testing (PBT). Instead of writing static test cases, you can define properties that should hold true for a wide range of automatically generated inputs.

## Installation

Add the `Prova.FsCheck` package to your test project:

```bash
dotnet add package Prova.FsCheck
```

## Creating Property Tests

Decorate your test methods with the `[Property]` attribute instead of `[Test]` or `[Fact]`. Prova will detect the parameters and use FsCheck to generate random inputs.

```csharp
using Prova.FsCheck;
using System.Linq;

public class MathTests
{
    [Property]
    public void AdditionIsCommutative(int a, int b)
    {
        if (a + b != b + a)
            throw new Exception("Commutativity broken!");
    }

    [Property]
    public void ReversingListTwiceYieldsOriginal(List<int> list)
    {
        var reversed = Enumerable.Reverse(list).ToList();
        var doubleReversed = Enumerable.Reverse(reversed).ToList();
        
        if (!list.SequenceEqual(doubleReversed))
             throw new Exception("Double reverse failed");
    }
}
```

## Configuration

You can configure FsCheck runner parameters directly on the attribute:

```csharp
[Property(MaxTest = 1000, MaxFail = 100)]
public void ExtensiveTest(int a) { ... }
```

Supported parameters:
- `MaxTest`: The maximum number of tests to run (default: 100).
- `MaxFail`: The maximum number of tests where inputs are discarded (default: 1000).
- `StartSize`: The size of the first test (default: 1).
- `EndSize`: The size of the last test (default: 100).
- `Verbose`: Set to `true` to output every generated input (useful for debugging).
- `QuietOnSuccess`: Suppress output on success.

## Failure Reporting

When a property fails, Prova captures the precise input that caused the failure (and shrinking information) and reports it as a test failure.

```
Failed: MathTests.AdditionIsCommutative
Exception: Falsifiable, after 1 tests (0 shrinks) :
(1, 2)
```

## Async Properties

Prova supports `async` property methods. Note that FsCheck drives the execution loop, so Prova handles the sync-over-async adaption automatically for property tests.

```csharp
[Property]
public async Task AsyncOperationHolds(int value)
{
    await Service.ProcessAsync(value);
}
```
