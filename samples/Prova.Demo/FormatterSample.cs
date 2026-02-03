using Prova;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class HexFormatter : IArgumentFormatter
    {
        public string Format(object? value)
        {
            if (value is int i) return $"0x{i:X}";
            if (value is byte b) return $"0x{b:X2}";
            return value?.ToString() ?? "null";
        }
    }

    public class FormatterSample
    {
        [Fact]
        [DisplayName("Value {0} is Hex {0}")]
        public void MatrixHexDisplay([Matrix(10, 255)] [ArgumentDisplayFormatter(typeof(HexFormatter))] int value)
        {
             // Test logic
        }

        public static IEnumerable<object[]> GetInts()
        {
            yield return new object[] { 15 };
            yield return new object[] { 1024 };
        }

        [Theory]
        [MemberData(nameof(GetInts))]
        [DisplayName("Hex Check: {0}")]
        public void MemberDataHexDisplay([ArgumentDisplayFormatter(typeof(HexFormatter))] int value)
        {
             // Test logic
        }
    }
}
