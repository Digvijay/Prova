using System.Threading.Tasks;
using Prova;
using Prova.Playwright.Sample;

namespace Prova.Playwright.Sample
{
    public partial class Program
{
    public static async Task Main(string[] args)
    {
        await TestRunnerExecutor.RunAllAsync(args);
    }
}
}
