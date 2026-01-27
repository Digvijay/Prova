namespace Prova.Core.Tests
{
    /// <summary>
    /// Entry point for tests.
    /// </summary>
    public static class Program
    {
        /// <summary>Main method.</summary>
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            await Prova.Generated.TestRunnerExecutor.RunAllAsync(args);
        }
    }
}
