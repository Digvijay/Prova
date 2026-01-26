namespace Prova.Core.Tests
{
    public static class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            await Prova.Generated.TestRunnerExecutor.RunAllAsync(args);
        }
    }
}
