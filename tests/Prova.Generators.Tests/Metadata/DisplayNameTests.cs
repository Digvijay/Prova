using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Prova.Generators.Analysis;
using Prova.Generators.Emission;
using Xunit;

namespace Prova.Generators.Tests
{
    public class DisplayNameTests
    {
        [Fact]
        public void DisplayName_Static_Text_Generates_Correctly()
        {
            var source = @"
using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class DisplayNameSample
    {
        [Fact]
        [DisplayName(""Custom Test Name"")]
        public void Test1() { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "DisplayName = \"Custom Test Name\"");
        }

        [Fact]
        public void DisplayName_With_Arguments_Generates_Format_Call()
        {
            var source = @"
using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class DisplayNameSample
    {
        [Theory]
        [InlineData(1, ""A"")]
        [DisplayName(""Value {0} is {1}"")]
        public void Test2(int i, string s) { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "DisplayName = string.Format(\"Value {0} is {1}\", i, s)");
        }

        [Fact]
        public void DisplayName_With_Matrix_Generates_Format_Call()
        {
            var source = @"
using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class DisplayNameSample
    {
        [Fact]
        [DisplayName(""User {0} is Active: {1}"")]
        public void Test3([Matrix(""Alice"", ""Bob"")] string name, [Matrix(true, false)] bool active) { }
    }
}";
            // Because of matrix, parameters are renamed to p0_name, p1_active etc.
            GeneratorVerifier.VerifyContains(source, "DisplayName = string.Format(\"User {0} is Active: {1}\", p0_name, p1_active)");
        }
    }
}
