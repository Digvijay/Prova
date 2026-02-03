using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class GlobalPoliciesTests
    {
        [Fact]
        public void Assembly_Retry_Applies_To_Test_Without_Attribute()
        {
            var source = @"
using Prova;

[assembly: Retry(3)]

public class AssemblyRetryTests
{
    [Fact]
    public void Test() { }
}";
            var expected = @"RetryCount = 3,";
            GeneratorVerifier.VerifyContains(source, expected);
        }

        [Fact]
        public void Method_Retry_Overrides_Assembly_Retry()
        {
            var source = @"
using Prova;

[assembly: Retry(3)]

public class AssemblyRetryOverrideTests
{
    [Fact]
    [Retry(5)]
    public void Test() { }
}";
            var expected = @"RetryCount = 5,";
            GeneratorVerifier.VerifyContains(source, expected);
        }

        [Fact]
        public void Class_Timeout_Overrides_Assembly_Timeout()
        {
            var source = @"
using Prova;

[assembly: Timeout(1000)]

[Timeout(500)]
public class AssemblyTimeoutOverrideTests
{
    [Fact]
    public void Test() { }
}";
            // SourceEmitter converts timeout to int?
            // "Timeout = 500,"
            var expected = @"Timeout = 500,";
            GeneratorVerifier.VerifyContains(source, expected);
        }

        [Fact]
        public void Assembly_Parallel_Is_Respected()
        {
            var source = @"
using Prova;

[assembly: Parallel(4)]

public class AssemblyParallelTests
{
    [Fact]
    public void Test() { }
}";
            // "MaxParallel = 4,"
            var expected = @"MaxParallel = 4,";
            GeneratorVerifier.VerifyContains(source, expected);
        }

        [Fact]
        public void Assembly_Sequential_Is_Respected()
        {
             var source = @"
using Prova;

[assembly: Sequential]

public class AssemblySequentialTests
{
    [Fact]
    public void Test() { }
}";
            var expected = @"MaxParallel = 1,";
            GeneratorVerifier.VerifyContains(source, expected);
        }
    }
}
