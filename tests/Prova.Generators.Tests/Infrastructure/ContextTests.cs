using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class ContextTests
    {
        [Fact]
        public void TestContext_Is_Initialized_In_RunAsync()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    
    public class MyTests
    {
        [Fact]
        public void Test1() {}
    }
}";
            // Verify TestContext.Current assignment
            GeneratorVerifier.VerifyContains(source, "Prova.TestContext.Current = context;");
        }
    }
}
