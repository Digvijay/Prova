# Code Coverage

Prova includes built-in code coverage that works seamlessly with Native AOT. No external tools or symbol-based instrumentation required.

## Quick Start

Run your tests with the `--coverage` flag:

```bash
dotnet run -- --coverage
```

This generates a `coverage.lcov` file in the current directory.

## How It Works

Prova's source generator injects lightweight "hit probes" into each test:

1. **Registration**: Each test case gets a unique `CoverageId`
2. **Tracking**: At test execution start, `CoverageRegistry.Hit(id)` is called
3. **Reporting**: After all tests complete, LCOV data is emitted

```csharp
// Generated code (simplified)
ExecuteDelegate = async () => {
    CoverageRegistry.Hit(0); // Probe injection
    // ... test execution
}
```

## LCOV Output Format

The generated `coverage.lcov` follows the standard LCOV format:

```
TN:
SF:GeneratedTests.cs
DA:1,1
DA:2,0
LF:2
LH:1
end_of_record
```

- `DA:<line>,<hits>` — Data line showing hit count
- `LF` — Lines Found (total)
- `LH` — Lines Hit (executed)

## Viewing Reports

Generate HTML reports using `genhtml`:

```bash
genhtml coverage.lcov --output-directory coverage-report
open coverage-report/index.html
```

Or use VS Code extensions like "Coverage Gutters" to visualize in-editor.

## AOT Compatibility

Unlike traditional coverage tools that rely on:
- PDB/debug symbols
- Runtime instrumentation
- IL rewriting

Prova's coverage is **source-level**, making it fully compatible with Native AOT compilation.

## Limitations

Current implementation tracks test method execution, not line-by-line coverage of application code. Future versions will support:
- Application code instrumentation
- Branch coverage
- Function coverage
