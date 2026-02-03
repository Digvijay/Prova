using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class FilterTests
    {
        [Fact]
        public void Filter_By_FullName_Emits_Condition()
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
            // Verify GetTests includes FullName
            GeneratorVerifier.VerifyContains(source, "FullName = \"Prova.Generators.Tests.MyTests.Test1\"");
        }
    }
}
