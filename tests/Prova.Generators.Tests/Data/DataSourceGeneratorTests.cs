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

            // The generator inlines the attribute construction and fully qualifies the type.
            GeneratorVerifier.VerifyContains(source, "new Prova.Generators.Tests.MyDataAttribute()");
            GeneratorVerifier.VerifyContains(source, "foreach (var dataRow in");
        }
    }
}
