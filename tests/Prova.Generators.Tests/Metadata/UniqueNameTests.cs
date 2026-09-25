namespace Prova.Generators.Tests
{
    /// <summary>
    /// Pins the structural name the generator emits alongside the display name.
    /// </summary>
    /// <remarks>
    /// The display name is a label. It may be overridden, it may be identical across every row of
    /// a theory, and it must therefore never be a test's identity. The generator emits a separate
    /// structural name for that purpose, always, whether or not a <c>[DisplayName]</c> is present.
    /// </remarks>
    public class UniqueNameTests
    {
        /// <summary>Every registration carries a structural name.</summary>
        [Fact]
        public void A_Fact_Gets_A_Structural_Name()
        {
            var source = @"
using Prova;

namespace Prova.Demo
{
    public class UniqueNameSample
    {
        [Fact]
        public void Test1() { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "UniqueName = $\"Prova.Demo.UniqueNameSample.Test1\"");
        }

        /// <summary>A display name override does not affect identity.</summary>
        [Fact]
        public void A_Custom_DisplayName_Does_Not_Become_The_Structural_Name()
        {
            var source = @"
using Prova;

namespace Prova.Demo
{
    public class UniqueNameSample
    {
        [Fact]
        [DisplayName(""Something entirely different"")]
        public void Test1() { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "DisplayName = \"Something entirely different\"");
            GeneratorVerifier.VerifyContains(source, "UniqueName = $\"Prova.Demo.UniqueNameSample.Test1\"");
        }

        /// <summary>Rows are distinguished structurally, not by label.</summary>
        [Fact]
        public void Theory_Rows_Get_Distinct_Structural_Names_Despite_A_Shared_DisplayName()
        {
            // This is the regression. The display name has no format placeholders, so both rows
            // are labelled identically; only the structural name tells them apart.
            var source = @"
using Prova;

namespace Prova.Demo
{
    public class UniqueNameSample
    {
        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 4)]
        [DisplayName(""Adds numbers"")]
        public void Add(int a, int b) { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "UniqueName = $\"Prova.Demo.UniqueNameSample.Add(1, 2)\"");
            GeneratorVerifier.VerifyContains(source, "UniqueName = $\"Prova.Demo.UniqueNameSample.Add(3, 4)\"");
        }
    }
}
