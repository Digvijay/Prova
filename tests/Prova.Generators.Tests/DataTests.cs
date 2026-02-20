using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class DataTests
    {
        [Fact]
        public void ClassData_Generates_Loop()
        {
            var source = @"
using Prova;
using System.Collections;
using System.Collections.Generic;

public class MyData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1 };
        yield return new object[] { 2 };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class MyDataTests
{
    [Theory]
    [ClassData(typeof(MyData))]
    public void DataTest(int x)
    {
    }
}";

            var expectedSnippets = new[] {
                "foreach (var dataRow in (global::System.Collections.Generic.IEnumerable<object[]>)new MyData())",
                "list.Add(new ProvaTest",
                "MyDataTests.DataTest",
                "instance.DataTest((int)dataRow[0])"
            };
            
            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }
    }
}
