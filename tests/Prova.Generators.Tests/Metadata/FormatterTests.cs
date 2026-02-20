using Prova.Generators.Tests;
using Xunit;

namespace Prova.Generators.Tests
{
    public class FormatterTests
    {
        [Fact]
        public void Matrix_With_Formatter_Generates_Format_Call()
        {
            var source = @"
using Prova;
using System;

namespace Prova.Demo
{
    public class HexFormatter : IArgumentFormatter
    {
        public string Format(object value) => $""0x{value:X}"";
    }

    public class FormatterSample
    {
        [Fact]
        public void Test1([Matrix(10, 255)] [ArgumentDisplayFormatter(typeof(HexFormatter))] int value) { }
    }
}";
            
            // Should contain new HexFormatter().Format(p0_value)
            // The method name parameter will be p0_value because it's a matrix
            GeneratorVerifier.VerifyContains(source, "new Prova.Demo.HexFormatter().Format(p0_value)");
        }


        [Fact]
        public void MemberData_With_Formatter_Generates_Format_Call()
        {
             var source = @"
using Prova;
using System;
using System.Collections.Generic;

namespace Prova.Demo
{
    public class HexFormatter : IArgumentFormatter
    {
        public string Format(object value) => $""0x{value:X}"";
    }

    public class FormatterSample
    {
        public static IEnumerable<object[]> GetData() { yield return new object[] { 15 }; }

        [Theory]
        [MemberData(nameof(GetData))]
        [DisplayName(""Value is {0}"")]
        public void Test3([ArgumentDisplayFormatter(typeof(HexFormatter))] int value) { }
    }
}";
            // In MemberData block, we loop and cast.
            // We expect: new Prova.Demo.HexFormatter().Format((int)dataRow[0])
            GeneratorVerifier.VerifyContains(source, "new Prova.Demo.HexFormatter().Format((int)dataRow[0])");
            
            // And verify it is used in string.Format
            GeneratorVerifier.VerifyContains(source, "string.Format(\"Value is {0}\""); 
            // The 2nd arg to string.Format is implied by the comma-separated list constructed from formattingArgsList
        }
    }
}
