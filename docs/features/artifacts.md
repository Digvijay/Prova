# Artifacts & Observability

Prova provides a unified way to capture and report output, logs, and artifacts (files) from your tests. This system works seamlessly across individual tests and global lifecycle hooks.

## Test Output & Attachments

Each test has access to an `ITestOutput` instance via `TestContext.Current.Output`. You can use this to:

- Write log messages: `TestContext.Current.Output.WriteLine("message")`
- Attach files: `TestContext.Current.Output.AttachArtifact("path/to/file.png", "Screenshot")`

This information is captured and reported by the test runner (and eventually CI systems).

## Session Artifacts & Global Hooks

Global hooks like `[BeforeAssembly]`, `[AfterAssembly]`, `[BeforeEvery]`, etc., also run within a valid `TestContext`.
- **Session Context**: Code running in Assembly-level hooks operates in a global "Session" context. Usage is identical:
  ```csharp
  [BeforeAssembly]
  public static void Setup()
  {
      TestContext.Current.Output.WriteLine("Global Setup...");
      TestContext.Current.Output.AttachArtifact("global-config.json");
  }
  ```
- **Context Stacking**: Prova automatically handles context saving and restoring. An `[AfterAssembly]` hook will have access to the same Session Context as `[BeforeAssembly]`.
