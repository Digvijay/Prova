using System.Threading.Tasks;
// Test definitions use Prova's own [Fact]/[Theory], dogfooding the framework under test.

namespace Prova.Generators.Tests
{
    public class GovernanceTests
    {
        [Fact]
        public void MaxAlloc_Attribute_Generates_AllocationCheck()
        {
            var source = @"
using Prova;
using System;

public class MyTests
{
    [Fact]
    [MaxAlloc(1024)]
    public void AllocationTest()
    {
    }
}";

            var expectedSnippets = new[] {
                "long maxAlloc = 1024;",
                "long startAlloc = global::System.GC.GetAllocatedBytesForCurrentThread();",
                "instance.AllocationTest();",
                "long endAlloc = global::System.GC.GetAllocatedBytesForCurrentThread();",
                "if (endAlloc - startAlloc > maxAlloc) throw new global::System.Exception($\"Memory allocation exceeded: {endAlloc - startAlloc} > {maxAlloc}\");"
            };

            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }
    }
}
