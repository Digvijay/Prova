# Debugging Support

Prova provides first-class support for debugging CI failures through automatic crash and hang dump generation, powered by the Microsoft Testing Platform (MTP).

## Crash Dumps

Automatically capture a process dump when the test host crashes (e.g., due to a segmentation fault, stack overflow, or unhandled exception).

### Usage

Enable crash dumps via the CLI:

```bash
dotnet test --crashdump
```

Or when running the executable directly:

```bash
./Tests --crashdump
```

### Configuration

You can configure dump type and other settings using standard MTP environment variables or CLI options if supported by the extension (refer to [Microsoft.Testing.Extensions.CrashDump documentation](https://learn.microsoft.com/dotnet/core/testing/unit-testing-platform-extensions#crash-dump)).

## Hang Dumps

Automatically capture a dump when a test execution exceeds a specified timeout.

### Usage

Enable hang dumps and specify the timeout:

```bash
dotnet test --hangdump --hangdump-timeout 10m
```

- `--hangdump`: Enables the hang dump provider.
- `--hangdump-timeout <value>`: Specifies the timeout duration (e.g., `10m`, `30s`). If the test execution takes longer than this, a dump is triggered and the process is terminated.

### Example

```bash
# Capture a dump if tests take longer than 5 minutes
dotnet test --hangdump --hangdump-timeout 5m
```

## Analyizing Dumps

Generated dumps are typically stored in the `TestResults` directory. You can analyze them using:
- **Visual Studio**: Open the `.dmp` file.
- **lldb** (Linux/macOS): `lldb --core <dumpfile> <executable>`
- **dotnet-dump**: `dotnet-dump analyze <dumpfile>`
