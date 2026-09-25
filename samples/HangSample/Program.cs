using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Samples.Hang
{
    /// <summary>
    /// Entry point for a sample that hangs on purpose, so that the hang dump provider has
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

    /// <summary>A test that does not finish promptly.</summary>
    public class HangTests
    {
        /// <summary>Waits long enough for the hang dump provider to trigger.</summary>
        [Fact]
        public async Task WillHang()
        {
            Console.WriteLine("About to hang...");
            await Task.Delay(TimeSpan.FromSeconds(30)).ConfigureAwait(false);
        }
    }
}
