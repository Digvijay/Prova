using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class LifecycleHookTests
    {
        /// <summary>
        /// [BeforeAll] derives from [Before] with HookScope.Class, so it must register a
        /// class-scoped hook. The generator previously matched hook attributes by exact type
        /// name, so every derived alias ([BeforeAll], [BeforeEach], [AfterAll], [AfterEach])
        /// compiled fine and then silently never ran.
        /// </summary>
        [Fact]
        public void BeforeAll_Generates_ClassHook()
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
            GeneratorVerifier.VerifyContains(source, "ClassBefore = new Func<Task>[] { () => { Prova.Generators.Tests.MyTests.GlobalSetup(); return Task.CompletedTask; } }");
        }

        /// <summary>
        /// [AfterAll] is the class-scoped counterpart and was dropped by the same defect.
        /// </summary>
        [Fact]
        public void AfterAll_Generates_ClassHook()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    
    public class MyTests
    {
        [AfterAll]
        public static void GlobalTeardown() {}
        
        [Fact]
        public void Test() {}
    }
}";
            GeneratorVerifier.VerifyContains(source, "ClassAfter = new Func<Task>[] { () => { Prova.Generators.Tests.MyTests.GlobalTeardown(); return Task.CompletedTask; } }");
        }

        /// <summary>
        /// [BeforeEach] / [AfterEach] are the test-scoped aliases, dropped by the same defect.
        /// </summary>
        [Fact]
        public void BeforeEach_And_AfterEach_Generate_TestHooks()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    
    public class MyTests
    {
        [BeforeEach]
        public void Setup() {}
        
        [AfterEach]
        public void Teardown() {}
        
        [Fact]
        public void Test() {}
    }
}";
            GeneratorVerifier.VerifyContains(source, "instance.Setup();");
            GeneratorVerifier.VerifyContains(source, "instance.Teardown();");
        }
    }
}
