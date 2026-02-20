# Command Line Interface 🚀

Prova tests are standard .NET Console Applications. You can run them using `dotnet run`, `dotnet test`, or by executing the compiled binary directly.

## Running Tests

### Standard Execution
```bash
dotnet test
```

### Direct Execution (Faster)
Executing the binary directly bypasses MSBuild overhead, providing near-instant startup.
```bash
./bin/Debug/net10.0/MyTestProject
```

---

## Prova-Specific Arguments

In Prova, arguments are passed after a `--` separator when using `dotnet run` or `dotnet test`.

### Filtering

| Argument | Description |
|----------|-------------|
| `--filter <PATTERN>` | Run tests matching the glob pattern (e.g., `*Calculator*`). |
| `--property <KEY>=<VALUE>` | Run tests with specific metadata traits. |
| `--focus` | Run only tests decorated with the `[Focus]` attribute. |

### Observability

| Argument | Description |
|----------|-------------|
| `--coverage` | Enable code coverage collection (Native LCOV). |
| `--coverage-output <PATH>` | Specify the output path for `coverage.lcov`. |
| `--list-tests` | List all discovered tests without executing them. |
| `--list-properties` | List all available properties/traits used in the project. |

### Debugging & Reliability

| Argument | Description |
|----------|-------------|
| `--crashdump` | Capture a process dump if the test runner crashes. |
| `--hangdump` | Capture a dump if tests exceed the hang timeout. |
| `--hangdump-timeout <DURATION>` | Set the hang timeout (e.g., `5m`, `1h`). |
| `--retry <COUNT>` | Override the default retry count for all tests. |
| `--timeout <MS>` | Override the default timeout for all tests. |

### Output & Logging

| Argument | Description |
|----------|-------------|
| `--verbose` | Enable detailed logging. |
| `--quiet` | Minimize console output. |
| `--no-color` | Disable ANSI color codes in the console. |
| `--report <FORMAT>` | Specify a report format (e.g., `console`, `json`). |

---

## Microsoft Testing Platform (MTP) Arguments

When running via `dotnet test`, you can also use standard MTP arguments:

```bash
# Generate a TRX report
dotnet test -- --report-trx

# Filter by FullyQualifiedName
dotnet test -- --filter "FullyQualifiedName~Calculator"
```
