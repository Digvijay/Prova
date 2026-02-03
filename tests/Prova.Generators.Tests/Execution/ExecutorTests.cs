using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class ExecutorTests
    {
        [Fact]
        public void CustomExecutor_Generates_Wrapper()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    using System.Threading.Tasks;

    public class MyExecutor : ITestExecutor
    {
        public Task ExecuteAsync(ProvaTest test, Func<Task> next) => next();
    }
    
    public class MyTests
    {
        [Fact]
        [Executor(typeof(MyExecutor))]
        public void Test() {}
    }
}";

            // Verify executor resolution and wrapping
            GeneratorVerifier.VerifyContains(source, "var executor = TestRunnerExecutor.Services.Get<MyExecutor>();");
            GeneratorVerifier.VerifyContains(source, "await executor.ExecuteAsync(test, async () =>");
        }
    }
}
