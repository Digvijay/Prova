using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class DataSourceGeneratorTests
    {
        [Fact]
        public void CustomGenerator_Generates_Loop()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    using System.Collections.Generic;
    
    public class MyDataAttribute : DataSourceGeneratorAttribute
    {
        public override IEnumerable<object[]> GetData(System.Reflection.MethodInfo method) => new[] { new object[] { 1 } };
    }
    
    public class MyTests
    {
        [Theory]
        [MyData]
        public void Test(int x) {}
    }
}";

            // Verify attribute instantiation and GetData call
            GeneratorVerifier.VerifyContains(source, "var attr = new MyDataAttribute();");
            GeneratorVerifier.VerifyContains(source, "foreach (var dataRow in attr.GetData(targetMethod))");
        }
    }
}
