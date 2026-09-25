namespace Prova.Generators.Tests
{
    /// <summary>
    /// Guards the generated runner's process exit code.
    /// </summary>
    /// <remarks>
    /// The emitted runner used to discard the exit code on both execution paths: the MTP path
    /// dropped the value returned by <c>app.RunAsync()</c>, and the standalone path counted
    /// failures but never returned anything. <c>RunAllAsync</c> returned <c>Task</c>, so the
    /// generated top-level entry point always exited 0 and CI reported success on a failing
    /// suite. These tests fail if any link in that chain regresses.
    /// </remarks>
    public class ExitCodeTests
    {
        private const string Source = @"
namespace Prova.Generators.Tests
{
    using Prova;

    public class MyTests
    {
        [Fact]
        public void Test() {}
    }
}";

        [Fact]
        public void RunAllAsync_Returns_ExitCode()
        {
            GeneratorVerifier.VerifyContains(Source, "public static async global::System.Threading.Tasks.Task<int> RunAllAsync(string[]? args = null)");
        }

        [Fact]
        public void RunAllAsync_Propagates_Both_Paths()
        {
            GeneratorVerifier.VerifyContains(Source, "return await RunMtpAsync(filteredArgs, hasCoverage);");
            GeneratorVerifier.VerifyContains(Source, "return await RunSimpleAsync(filteredArgs, hasCoverage);");
        }

        [Fact]
        public void MtpPath_Returns_Platform_ExitCode()
        {
            GeneratorVerifier.VerifyContains(Source, "int exitCode = await app.RunAsync();");
            GeneratorVerifier.VerifyContains(Source, "return exitCode;");
        }

        [Fact]
        public void SimplePath_Returns_NonZero_When_Tests_Failed()
        {
            GeneratorVerifier.VerifyContains(Source, "return failed > 0 ? 1 : 0;");
        }
    }
}
