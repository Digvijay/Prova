using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class TimeoutTests
    {
        [Fact]
        public void Timeout_Attribute_Generates_TaskWhenAny()
        {
            var source = @"
using Prova;
using System;
using System.Threading.Tasks;

public class MyTimeoutTests
{
    [Fact]
    [Timeout(1000)]
    public async Task SlowTest()
    {
        await Task.Delay(2000);
    }
}";

            var expectedSnippets = new[] {
                "Timeout = 1000",
                "instance = new MyTimeoutTests()",
                "await instance.SlowTest()"
            };
            
            GeneratorVerifier.VerifyContains(source, expectedSnippets);
        }
    }
}
