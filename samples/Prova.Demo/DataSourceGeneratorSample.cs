using System;
using System.Collections.Generic;
using Prova;

namespace Prova.Demo
{
    public class RangeDataSourceAttribute : DataSourceGeneratorAttribute
    {
        private readonly int _start;
        private readonly int _count;

        public RangeDataSourceAttribute(int start, int count)
        {
            _start = start;
            _count = count;
        }

        public override IEnumerable<object?[]> GetData()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return new object?[] { _start + i };
            }
        }
    }

    public class DataSourceGeneratorSample
    {
        [Theory]
        [RangeDataSource(10, 5)]
        public void Test_CustomRange(int value)
        {
            Console.WriteLine($"Testing with value: {value}");
            Assert.True(value >= 10 && value < 15);
        }

        [Theory]
        [RangeDataSource(1, 3)]
        [RangeDataSource(100, 2)]
        public void Test_MultipleCustomRanges(int value)
        {
            Console.WriteLine($"Testing multiple ranges with value: {value}");
            Assert.True((value >= 1 && value < 4) || (value >= 100 && value < 102));
        }
    }

    [RangeDataSource(50, 2)]
    public class ClassLevelGeneratorSample
    {
        private readonly int _classData;

        public ClassLevelGeneratorSample(int classData)
        {
            _classData = classData;
        }

        [Fact]
        public void Test_ClassLevelData()
        {
            Console.WriteLine($"Testing class-level data: {_classData}");
            Assert.True(_classData == 50 || _classData == 51);
        }
    }
}
