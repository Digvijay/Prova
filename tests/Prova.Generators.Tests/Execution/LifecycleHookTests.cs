using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class LifecycleHookTests
    {
        [Fact]
        public void BeforeAll_Generates_AssemblyHook()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    
    public class MyTests
    {
        [BeforeAll]
        public static void GlobalSetup() {}
        
        [Fact]
        public void Test() {}
    }
}";
            // Hooks are captured in RunAsync entry point
            GeneratorVerifier.VerifyContains(source, "await TestRunnerExecutor.InvokeHookHelper(() => { MyTests.GlobalSetup(); return global::System.Threading.Tasks.Task.CompletedTask; }, \"MyTests.GlobalSetup\", null);");
        }
    }
}
