using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class CombinatorialTests
    {
        [Fact]
        public void Matrix_Generates_Loop()
        {
            var source = @"
using Prova;

public class MatrixTests
{
    [Fact]
    public void Test1([Matrix(1, 2, 3)] int x) { }
}";
            
            // Verify loop generation
            GeneratorVerifier.VerifyContains(source, "foreach (var p0_x in new [] { 1, 2, 3 })");
            GeneratorVerifier.VerifyContains(source, "Test1(p0_x)");
        }

        [Fact]
        public void Matrix_Generates_Nested_Loops()
        {
            var source = @"
using Prova;

public class MatrixTests
{
    [Fact]
    public void Test2([Matrix(1, 2)] int x, [Matrix(true, false)] bool y) { }
}";
            
            // Verify nested loops
            GeneratorVerifier.VerifyContains(source, "foreach (var p0_x in new [] { 1, 2 })");
            GeneratorVerifier.VerifyContains(source, "foreach (var p1_y in new [] { true, false })");
            GeneratorVerifier.VerifyContains(source, "Test2(p0_x, p1_y)");
        }

        [Fact]
        public void Matrix_With_Strings_Are_Escaped()
        {
            var source = @"
using Prova;

public class MatrixTests
{
    [Fact]
    public void Test3([Matrix(""a"", ""b"")] string s) { }
}";
            
            // Verify string escaping
            GeneratorVerifier.VerifyContains(source, "foreach (var p0_s in new [] { \"a\", \"b\" })");
            GeneratorVerifier.VerifyContains(source, "Test3(p0_s)");
        }
    }
}
