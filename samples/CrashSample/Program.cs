using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Samples.Crash
{
    /// <summary>
    /// Entry point for a sample that crashes on purpose, so that the crash dump provider has
    /// something to capture.
    /// </summary>
    public static class Program
    {
        /// <summary>Runs the sample.</summary>
        /// <param name="args">The process arguments.</param>
        /// <returns>The runner's exit code.</returns>
        public static Task<int> Main(string[] args)
            => Prova.TestRunnerExecutor.RunAllAsync(args);
    }

    /// <summary>A test that terminates the process.</summary>
    public class CrashTests
    {
        /// <summary>Fails fast, which the crash dump provider is expected to capture.</summary>
        [Fact]
        public void WillCrash()
        {
            Console.WriteLine("About to crash...");
            Environment.FailFast("Intentional crash, used to exercise dump generation.");
        }
    }
}
