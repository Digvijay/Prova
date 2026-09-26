using System.Threading.Tasks;
using Prova;
using Prova.Playwright.Sample;

namespace Prova.Playwright.Sample
{
    public partial class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await TestRunnerExecutor.RunAllAsync(args);
        }
    }
}
