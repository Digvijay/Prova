using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class ClassFactoryTests
    {
        [Fact]
        public void ClassFactory_Generates_CustomInstantiation()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    using System;
    
    public class MyFactory : IClassConstructor<MyTests>
    {
        public MyTests CreateInstance(IServiceProvider services) => new MyTests();
    }
    
    [ClassFactory(typeof(MyFactory))]
    public class MyTests
    {
        [Fact]
        public void Test() {}
    }
}";

            // Verify factory resolution from DI and CreateInstance call.
            // The generator emits fully-qualified type names to avoid ambiguity in user code.
            GeneratorVerifier.VerifyContains(source, "var factory = TestRunnerExecutor.Services.Get<Prova.Generators.Tests.MyFactory>();");
            GeneratorVerifier.VerifyContains(source, "instance = factory.CreateInstance(TestRunnerExecutor.Services);");
        }

        [Fact]
        public void NoClassFactory_Generates_StandardInstantiation()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    
    public class MyTests
    {
        [Fact]
        public void Test() {}
    }
}";

            // Verify standard constructor IS used (fully qualified by the generator)
            GeneratorVerifier.VerifyContains(source, "instance = new Prova.Generators.Tests.MyTests();");
        }
    }
}
