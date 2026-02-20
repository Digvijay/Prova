using System.Linq;
using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class LifecycleTests
    {
        [Fact]
        public void Before_And_After_Generates_Calls()
        {
            var source = @"
using Prova;
using System.Collections.Generic;

public class MyTests
{
    public List<string> Log { get; } = new List<string>();

    [Before]
    public void Setup()
    {
        Log.Add(""Setup"");
    }

    [After]
    public void Teardown()
    {
        Log.Add(""Teardown"");
    }

    [Fact]
    public void Test1() 
    {
        Log.Add(""Test"");
    }
}";

            GeneratorVerifier.VerifyContains(source, "CoverageRegistry.Hit");
            GeneratorVerifier.VerifyContains(source, "await TestRunnerExecutor.InvokeHookHelper(() => { instance.Setup(); return global::System.Threading.Tasks.Task.CompletedTask; }, \"Setup\", null);");
            GeneratorVerifier.VerifyContains(source, "instance.Test1()");
        }

        [Fact]
        public void Class_Hooks_Generate_Delegates()
        {
            var source = @"
using Prova;
using System.Threading.Tasks;

public class ClassHooksTests
{
    [Before(HookScope.Class)]
    public static void GlobalSetup() { }

    [After(HookScope.Class)]
    public static void GlobalTeardown() { }

    [Fact]
    public void Test1() { }
}";

            GeneratorVerifier.VerifyContains(source, "ClassHooksTests.GlobalSetup()");
            GeneratorVerifier.VerifyContains(source, "ClassHooksTests.GlobalTeardown()");
        }

        [Fact]
        public void Assembly_Hooks_Generate_Calls_In_RunAll()
        {
            var source = @"
using Prova;

public static class AssemblySetup
{
    [Before(HookScope.Assembly)]
    public static void Init() { }

    [After(HookScope.Assembly)]
    public static void Cleanup() { }
}

public class MyTests { [Fact] public void T1() {} }";

            GeneratorVerifier.VerifyContains(source, "InitializeCoverage()");
            GeneratorVerifier.VerifyContains(source, "await TestRunnerExecutor.InvokeHookHelper(() => { AssemblySetup.Init(); return global::System.Threading.Tasks.Task.CompletedTask; }, \"AssemblySetup.Init\", null);");
            GeneratorVerifier.VerifyContains(source, "await TestRunnerExecutor.InvokeHookHelper(() => { AssemblySetup.Cleanup(); return global::System.Threading.Tasks.Task.CompletedTask; }, \"AssemblySetup.Cleanup\", null);");
        }

        [Fact]
        public void Global_Hooks_Generate_Delegates_And_Await_Calls()
        {
            var source = @"
using Prova;
using Prova.Core.Attributes;
using System.Threading.Tasks;

public static class GlobalHooks
{
    [BeforeEvery(HookScope.Test)]
    public static void SyncBeforeTest() { }

    [BeforeEvery(HookScope.Class)]
    public static async Task AsyncBeforeClass() 
    {
        await Task.Delay(1);
    }
}

public class MyTests { [Fact] public void T1() {} }";

            // Verify delegate wrapper generation
            GeneratorVerifier.VerifyContains(source, "_beforeEveryTest = new List<Func<global::System.Threading.Tasks.Task>>()");
            GeneratorVerifier.VerifyContains(source, "Task.CompletedTask;");
            GeneratorVerifier.VerifyContains(source, "async () => await TestRunnerExecutor.InvokeHookHelper(async () => await GlobalHooks.AsyncBeforeClass(), \"GlobalHooks.AsyncBeforeClass\", null)");

            // Verify invocation
            GeneratorVerifier.VerifyContains(source, "await hook();");
        }
    }
}
