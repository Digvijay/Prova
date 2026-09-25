using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class DynamicTestDiscoveryTests
    {
        /// <summary>
        /// Per docs/dynamic-tests.md, [TestFactory] marks a <c>public static</c> method that
        /// accepts a <see cref="Prova.DynamicTestBuilder"/> and registers tests on it.
        /// </summary>
        [Fact]
        public void TestFactory_Generates_DynamicDiscovery()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    using System.Threading.Tasks;
    
    public class MyTests
    {
        [TestFactory]
        public static void MyFactory(DynamicTestBuilder builder)
        {
            builder.Add(""Dynamic1"", () => Task.CompletedTask);
        }
    }
}";

            // The builder is created once and handed to every discovered factory.
            GeneratorVerifier.VerifyContains(source, "var builder = new Prova.DynamicTestBuilder();");
            GeneratorVerifier.VerifyContains(source, "Prova.Generators.Tests.MyTests.MyFactory(builder);");
        }
    }
}
