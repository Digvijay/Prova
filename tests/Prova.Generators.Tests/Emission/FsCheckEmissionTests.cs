namespace Prova.Generators.Tests
{
    /// <summary>
    /// Covers emission for FsCheck's <c>[Property]</c> attribute.
    /// </summary>
    /// <remarks>
    /// These tests existed as an orphaned file under <c>src/Prova.Generators.Tests/</c> that had
    /// no project file, so they were never compiled or run. They also used a VerifyXunit
    /// snapshot API that this repository does not reference. Ported here against the real
    /// verifier so the emission is actually covered.
    /// </remarks>
    public class FsCheckEmissionTests
    {
        [Fact]
        public void Property_Generates_ForAll_Invocation()
        {
            var source = @"
using Prova.FsCheck;
using System;

namespace TestProject
{
    public class MyTests
    {
        [Property]
        public void MyProperty(int a, string b) { }
    }
}";

            GeneratorVerifier.VerifyContains(source, "global::FsCheck.Fluent.Prop.ForAll<int, string>");
            GeneratorVerifier.VerifyContains(source, "Prova.FsCheck.FsCheckRunner.Run(fsConfig, fsCheckProp);");
        }

        [Fact]
        public void Property_Config_Is_Forwarded_To_Runner()
        {
            var source = @"
using Prova.FsCheck;
using System;

namespace TestProject
{
    public class MyTests
    {
        [Property(MaxTest = 500, Verbose = true)]
        public void MyProperty(int a) { }
    }
}";

            GeneratorVerifier.VerifyContains(source, "fsConfig[\"MaxTest\"] = \"500\";");
            GeneratorVerifier.VerifyContains(source, "fsConfig[\"Verbose\"] = \"True\";");
        }
    }
}
