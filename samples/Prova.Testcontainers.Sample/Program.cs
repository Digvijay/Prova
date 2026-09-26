using System.Threading.Tasks;
using Prova;

namespace Prova.Testcontainers.Sample
{
    sealed class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await TestRunnerExecutor.RunAllAsync(args);
        }
    }
}
