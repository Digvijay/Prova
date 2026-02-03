using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class CoverageTests
    {
        [Fact]
        public void Coverage_Injection_Generates_Hit_Probes()
        {
            var source = @"
using Prova;

public class MyTests
{
    [Fact]
    public void Test1() {}
}";
            
            // Should contain the hit probe with ID 0
            GeneratorVerifier.VerifyContains(source, "CoverageRegistry.Hit(0);");
        }

        [Fact]
        public void Coverage_Registry_Initialization_Generated_In_Runner()
        {
            var source = @"
using Prova;

public class MyTests
{
    [Fact]
    public void Test1() {}
}";
            
            // Should contain the InitializeCoverage call
            GeneratorVerifier.VerifyContains(source, "InitializeCoverage();");
            
            // Should contain the metadata for the test
            GeneratorVerifier.VerifyContains(source, "var metadata = new string[1] {");
            GeneratorVerifier.VerifyContains(source, "\"MyTests.Test1\",");
        }

        [Fact]
        public void Coverage_Report_Emission_Generated_In_Main()
        {
            var source = @"
using Prova;

public class MyTests
{
    [Fact]
    public void Test1() {}
}";
            
            // Should contain the EmitLcov logic in RunSimpleAsync or RunMtpAsync
            GeneratorVerifier.VerifyContains(source, "if (hasCoverage)");
            GeneratorVerifier.VerifyContains(source, "CoverageRegistry.EmitLcov(\"coverage.lcov\");");
        }
    }
}
