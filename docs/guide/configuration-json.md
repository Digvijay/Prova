# Runtime Configuration (testconfig.json)

Prova supports runtime configuration through a `testconfig.json` file. This allows you to change execution policies and global metadata without recompiling your test assembly.

## Configuration File

Place a `testconfig.json` file in the root of your test project and ensure it is copied to the output directory (e.g., via `<None Update="testconfig.json" CopyToOutputDirectory="PreserveNewest" />`).

### Example File

```json
{
  "DefaultRetryCount": 2,
  "DefaultTimeoutMs": 5000,
  "MaxParallel": 8,
  "DefaultCulture": "en-GB",
  "GlobalProperties": {
    "Project": "MyEngine",
    "Tier": "P1"
  }
}
```

## Configuration Options

| Option | Type | Description |
| :--- | :--- | :--- |
| `DefaultRetryCount` | `int` | Number of times to retry failed tests globally. |
| `DefaultTimeoutMs` | `int` | Timeout in milliseconds for test execution. |
| `MaxParallel` | `int` | Maximum number of tests to run in parallel. Defaults to `Environment.ProcessorCount`. |
| `DefaultCulture` | `string` | The default culture (e.g., "en-US") for all tests. |
| `GlobalProperties` | `object` | Key-value pairs added to every test's metadata. |

## Hierarchy of Configuration

Prova follows a strict hierarchy for resolving execution policies. The first value found wins:

1.  **Method Level Attribute**: `[Retry(3)]` or `[Timeout(100)]` on the test method.
2.  **Class Level Attribute**: `[Retry(2)]` on the test class.
3.  **Assembly Level Attribute**: `[assembly: Retry(1)]`.
4.  **`testconfig.json`**: Values defined in the JSON file.
5.  **Build-in Defaults**: Retry = 0, Timeout = null, Repeat = 1.

## Native AOT Compatibility

The configuration loader is built using source-generated JSON serialization, making it fully compatible with Native AOT and trimmed applications.
