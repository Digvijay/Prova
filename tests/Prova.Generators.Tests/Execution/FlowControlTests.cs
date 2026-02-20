using Prova.Generators.Tests;
using Xunit;

namespace Prova.Generators.Tests
{
    public class FlowControlTests
    {
        [Fact]
        public void Repeat_Attribute_Generates_Loop_And_Count()
        {
            var source = @"
using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class RepeatSample
    {
        [Fact]
        [Repeat(3)]
        public void RepeatedTest() { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "Repeat = 3");
            GeneratorVerifier.VerifyContains(source, "for (int i = 0; i < repeatCount; i++)");
        }

        [Fact]
        public void Retry_Attribute_Generates_RetryCount()
        {
            var source = @"
using Prova;
using System;

namespace Prova.Demo
{
    public class FlowSample
    {
        [Fact]
        [Retry(3)]
        public void FlakyTest() { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "RetryCount = 3");
            GeneratorVerifier.VerifyContains(source, "if (attempt < retryCount)");
        }

        [Fact]
        public void Timeout_Attribute_Generates_Timeout_Logic()
        {
             var source = @"
using Prova;
using System;

namespace Prova.Demo
{
    public class FlowSample
    {
        [Fact]
        [Timeout(100)]
        public void SlowTest() { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "Timeout = 100");
            GeneratorVerifier.VerifyContains(source, "Task.Delay(timeout.Value)");
            GeneratorVerifier.VerifyContains(source, "throw new global::System.TimeoutException");
        }

        [Fact]
        public void Culture_Attribute_Generates_Switching_Logic()
        {
             var source = @"
using Prova;
using System;

namespace Prova.Demo
{
    public class FlowSample
    {
        [Fact]
        [Culture(""fr-FR"")]
        public void FrenchTest() { }
    }
}";
            GeneratorVerifier.VerifyContains(source, "Culture = \"fr-FR\"");
            GeneratorVerifier.VerifyContains(source, "CultureInfo.GetCultureInfo(culture)");
            GeneratorVerifier.VerifyContains(source, "CurrentCulture = newCulture");
        }
    }
}
