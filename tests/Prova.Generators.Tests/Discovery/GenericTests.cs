using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class GenericTests
    {
        [Fact]
        public void GenericClass_Generates_Variants()
        {
            var source = @"
using Prova;

[Variant(typeof(int))]
[Variant(typeof(string))]
public class MyGenericTests<T>
{
    [Fact]
    public void Test() { }
}";
            GeneratorVerifier.VerifyContains(source, "MyGenericTests<int>.Test");
            GeneratorVerifier.VerifyContains(source, "MyGenericTests<string>.Test");
        }

        [Fact]
        public void GenericMethod_Generates_Variants()
        {
            var source = @"
using Prova;

public class MyTests
{
    [Variant(typeof(int))]
    [Variant(typeof(double))]
    [Fact]
    public void GenericMethod<T>() { }
}";
            GeneratorVerifier.VerifyContains(source, "MyTests.GenericMethod<int>");
            GeneratorVerifier.VerifyContains(source, "MyTests.GenericMethod<double>");
        }

        [Fact]
        public void CartesianProduct_Generates_All_Variants()
        {
            var source = @"
using Prova;

[Variant(typeof(int))]
public class MyComplexTests<T1>
{
    [Variant(typeof(string))]
    [Variant(typeof(bool))]
    [Fact]
    public void Test<T2>() { }
}";
            GeneratorVerifier.VerifyContains(source, "MyComplexTests<int>.Test<string>");
            GeneratorVerifier.VerifyContains(source, "MyComplexTests<int>.Test<bool>");
        }
    }
}
