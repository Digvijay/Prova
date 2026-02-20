using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class ConcurrencyTests
    {
        [Fact]
        public void Parallel_Attribute_Generates_Semaphore()
        {
            var source = @"
using Prova;
using System;

public class MyParallelTests
{
    [Fact]
    [Parallel(2)]
    public void ParallelTest()
    {
    }
}";
            
            var expectedSnippets = new[] {
                "MaxParallel = 2",
                "instance = new MyParallelTests()",
                "instance.ParallelTest()"
            };
            
            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }
    }
}
