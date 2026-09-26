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

            // Executors are registered for AOT and referenced by type on the registration,
            // then resolved and wrapped around the test body at run time.
            GeneratorVerifier.VerifyContains(source, "ExecutorType = typeof(Prova.Generators.Tests.MyExecutor)");
            GeneratorVerifier.VerifyContains(source, "Services.AddTransient<Prova.Generators.Tests.MyExecutor>(() => new Prova.Generators.Tests.MyExecutor());");
        }
    }
}
