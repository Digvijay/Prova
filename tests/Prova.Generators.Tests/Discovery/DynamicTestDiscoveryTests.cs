using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class DynamicTestDiscoveryTests
    {
        [Fact]
        public void TestFactory_Generates_DynamicDiscovery()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    using System.Collections.Generic;
    
    public class MyTests
    {
        [TestFactory]
        public IEnumerable<DynamicTestBuilder> MyFactory()
        {
            yield return DynamicTestBuilder.Create(""Dynamic1"", () => Task.CompletedTask);
        }
    }
}";

            // Verify factory call in GetTests
            GeneratorVerifier.VerifyContains(source, "foreach (var dynamicTest in new MyTests().MyFactory())");
            GeneratorVerifier.VerifyContains(source, "yield return dynamicTest.Build();");
        }
    }
}
