using System.Threading.Tasks;
using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests.Attachments
{
    public class AttachmentTests
    {
        [Fact]
        public void AttachArtifact_GeneratesCorrectCalls()
        {
            var code = @"
using Prova;
using System.Threading.Tasks;

public class AttachmentTest
{
    [Fact]
    public void TestWithAttachment()
    {
        TestContext.Current.Output.AttachArtifact(""/tmp/log.txt"", ""My Log"", ""text/plain"");
    }
}";
            // Verify that we are capturing the output from the context
            GeneratorVerifier.VerifyContains(code, "stdOut = context.Output.Output ?? \"\";");
            
            // Verify that we are appending the output to the final result
            GeneratorVerifier.VerifyContains(code, "var finalOutput = (string.IsNullOrEmpty(lastOutput) ? \"\" : lastOutput + System.Environment.NewLine) + stdOut;");
        }
    }
}
