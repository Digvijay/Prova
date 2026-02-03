using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests
{
    public class PropertyTests
    {
        [Fact]
        public void Property_Is_Captured_In_Registration()
        {
            var source = @"
namespace Prova.Generators.Tests
{
    using Prova;
    
    public class MyTests
    {
        [Fact]
        [Property(""Category"", ""Fast"")]
        public void Test1() {}
    }
}";
            // Verify Registration includes Properties
            GeneratorVerifier.VerifyContains(source, "Properties = new global::System.Collections.Generic.Dictionary<string, string>");
            GeneratorVerifier.VerifyContains(source, "[\"Category\"] = \"Fast\"");
        }
    }
}
