using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class DataSourceTests
    {
        /// <summary>
        /// Per docs/data-sources.md, [ClassDataSource] takes the provider type only, and the
        /// provider must implement <c>IEnumerable&lt;object[]&gt;</c> so it can be resolved from DI.
        /// </summary>
        [Fact]
        public void ClassDataSource_Generates_Loop()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    using System.Collections;
    using System.Collections.Generic;
    
    public class MyData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator() { yield return new object[] { 1 }; }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class MyTests
    {
        [Theory]
        [ClassDataSource(typeof(MyData))]
        public void Test(int x) {}
    }
}";

            // The provider is resolved from the container and enumerated as object[] rows.
            GeneratorVerifier.VerifyContains(source, "TestRunnerExecutor.Services.Get<Prova.Generators.Tests.MyData>()");
            GeneratorVerifier.VerifyContains(source, "foreach (var dataRow in");
        }
    }
}
