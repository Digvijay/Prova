using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class DIDataSourceTests
    {
        [Fact]
        public void DIDataSource_Generates_DIRootedLoop()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    using System.Collections.Generic;
    
    public class MyDataProvider
    {
        public IEnumerable<object[]> GetData() => new[] { new object[] { 1 } };
    }
    
    public class MyTests
    {
        [Theory]
        [DependencyInjectionDataSource(typeof(MyDataProvider), nameof(MyDataProvider.GetData))]
        public void Test(int x) {}
    }
}";

            // Verify provider resolution from DI and method call
            GeneratorVerifier.VerifyContains(source, "var provider = TestRunnerExecutor.Services.Get<MyDataProvider>();");
            GeneratorVerifier.VerifyContains(source, "foreach (var dataRow in provider.GetData())");
        }
    }
}
