# Test Attachments

Prova allows you to attach artifacts (files, screenshots, logs, etc.) to your test results. This is useful for debugging failures in CI environments or providing additional context for test execution.

## Usage

You can attach artifacts using the `TestContext.Current.Output.AttachArtifact` method.

```csharp
using Prova;
using System.IO;

public class AttachmentTests
{
    [Fact]
    public void TestWithAttachment()
    {
        // ... perform test actions ...

        // Attach a file
        var logPath = Path.GetFullPath("app.log");
        File.WriteAllText(logPath, "Application execution log...");
        
        TestContext.Current.Output.AttachArtifact(logPath, "Application Log", "text/plain");

        // Attach a screenshot (example)
        TestContext.Current.Output.AttachArtifact("screenshot.png", "Failure Screenshot", "image/png");
    }
}
```

## Output Format

Attachments are reported in the test output using a structured format which can be parsed by compatible listeners/reporters:

```
[[ATTACHMENT|/absolute/path/to/file|Display Name|mime/type]]
```

## Reporting

The standard console reporter will display these attachments in the output. Extensions (like TrxReport) can potentialy capture these artifacts (future integration).
