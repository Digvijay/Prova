using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class DataSourceTests
    {
        [Fact]
        public void ClassDataSource_Generates_Loop()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    using System.Collections.Generic;
    
    public class MyData
    {
        public IEnumerable<object[]> GetData() => new[] { new object[] { 1 } };
    }
    
    [ClassDataSource(typeof(MyData), nameof(MyData.GetData))]
    public class MyTests
    {
        public MyTests(int x) {}
        
        [Fact]
        public void Test() {}
    }
}";

            // Verify data source loop around class instantiation
            GeneratorVerifier.VerifyContains(source, "var dataSource = new MyData();");
            GeneratorVerifier.VerifyContains(source, "foreach (var classDataRow in dataSource.GetData())");
        }
    }
}
