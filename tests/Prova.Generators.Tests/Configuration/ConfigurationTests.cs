using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void Method_Override_Takes_Precedence_Over_Class_Timeout()
        {
            var source = @"
using Prova;

[Timeout(1000)]
public class TimedClass
{
    [Fact]
    [Timeout(500)]
    public void TestMethod() {}
}";
            
            // Method level (500) should win
            GeneratorVerifier.VerifyContains(source, "Timeout = 500,");
        }

        [Fact]
        public void Class_Timeout_Is_Used_If_Method_Is_Default()
        {
            var source = @"
using Prova;

[Timeout(1000)]
public class TimedClass
{
    [Fact]
    public void TestMethod() {}
}";
            
            // Class level (1000) should be used
            GeneratorVerifier.VerifyContains(source, "Timeout = 1000,");
        }

        [Fact]
        public void Method_Override_Takes_Precedence_Over_Class_Retry()
        {
            var source = @"
using Prova;

[Retry(3)]
public class RetryingClass
{
    [Fact]
    [Retry(1)]
    public void TestMethod() {}
}";
            
            // Method level (1) should win
            GeneratorVerifier.VerifyContains(source, "RetryCount = 1,");
        }

        [Fact]
        public void Assembly_Timeout_Is_Last_Resort()
        {
            var source = @"
using Prova;
[assembly: Timeout(999)]

public class HelperClass
{
    [Fact]
    public void TestMethod() {}
}";
            
            // Assembly default
            GeneratorVerifier.VerifyContains(source, "Timeout = 999,");
        }

        [Fact]
        public void Class_Takes_Precedence_Over_Assembly_Timeout()
        {
            var source = @"
using Prova;
[assembly: Timeout(999)]

[Timeout(100)]
public class HelperClass
{
    [Fact]
    public void TestMethod() {}
}";
            
            // Class override
            GeneratorVerifier.VerifyContains(source, "Timeout = 100,");
        }
    }
}
