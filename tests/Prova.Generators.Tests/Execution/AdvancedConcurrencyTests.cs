using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Prova.Generators;
// using Prova.Generators.Tests.Infrastructure; // Removed based on check

namespace Prova.Generators.Tests
{
    public class AdvancedConcurrencyTests
    {
        [Fact]
        public void ParallelLimiters_Should_Limit_Concurrency()
        {
            var source = @"
using Prova;
using System.Threading.Tasks;

public class LimiterTests
{
    [Fact]
    [ParallelLimiter(""DB"", 2)]
    public async Task Test1() => await Task.Delay(100);

    [Fact]
    [ParallelLimiter(""DB"", 2)]
    public async Task Test2() => await Task.Delay(100);

    [Fact]
    [ParallelLimiter(""DB"", 2)]
    public async Task Test3() => await Task.Delay(100);
}";
            GeneratorVerifier.VerifyContains(source, "ParallelLimiters = new (string, int)[] { (\"DB\", 2) }");
            GeneratorVerifier.VerifyContains(source, "resourceSemaphores.GetOrAdd");
        }

        [Fact]
        public void ParallelGroup_Should_Be_Emitted()
        {
            var source = @"
using Prova;
using System.Threading.Tasks;

public class GroupTests
{
    [Fact]
    [ParallelGroup(""GroupA"")]
    public void TestA() { }
}";
            GeneratorVerifier.VerifyContains(source, "ParallelGroup = \"GroupA\"");
        }
    }
}
