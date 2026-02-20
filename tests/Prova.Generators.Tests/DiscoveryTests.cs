using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class DiscoveryTests
    {
        [Fact]
        public void Fact_Discovers_Test()
        {
            var source = @"
using Prova;
using System;

public class MyFactTests
{
    [Fact]
    public void SimpleFact()
    {
    }
}";
            var expectedSnippets = new[] {
                "MyFactTests.SimpleFact",
                "new MyFactTests()",
                "instance.SimpleFact()"
            };
            
            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }

        [Fact]
        public void Theory_Generates_InlineData_Registration()
        {
            var source = @"
using Prova;
using Xunit;

public class MyTheoryTests
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(3, 4)]
    public void Add(int a, int b) { }
}";
            var expectedSnippets = new[] {
                "MyTheoryTests.Add",
                "instance.Add(1, 2)",
                "instance.Add(3, 4)"
            };
            
            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }

        [Fact]
        public void Fact_With_Skip_Generates_SkipReason()
        {
            var source = @"
using Prova;
using Xunit;

public class MySkipTests
{
    [Fact(Skip = ""Not ready"")]
    public void SkippedTest() { }
}";
            var expectedSnippets = new[] {
                "SkipReason = \"Not ready\""
            };
            
            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }

        [Fact]
        public void Fact_With_Focus_Generates_Focus_Trait()
        {
            var source = @"
using Prova;
using Xunit;

public class MyFocusTests
{
    [Fact]
    [Focus]
    public void FocusedTest() { }
}";
            var expectedSnippets = new[] {
                "Properties = new Dictionary<string, string>",
                "{ \"Focus\", \"true\" }"
            };
            
            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }

        [Fact]
        public void Fact_With_Retry_Generates_RetryCount()
        {
            var source = @"
using Prova;
using Xunit;

public class MyRetryTests
{
    [Fact]
    [Retry(5)]
    public void RetriedTest() { }
}";
            var expectedSnippets = new[] {
                "RetryCount = 5"
            };
            
            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }
    }
}
