using Prova;
using System.Threading.Tasks;

namespace Prova.Testcontainers.Sample
{
    sealed class Program
{
    static async Task Main(string[] args)
    {
        await TestRunnerExecutor.RunAllAsync(args);
    }
}
}
