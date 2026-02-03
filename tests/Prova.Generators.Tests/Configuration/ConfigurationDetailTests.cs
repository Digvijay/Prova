namespace Prova.Generators.Tests
{
namespace Prova.Generators.Tests
{
    using Prova.Generators.Tests;

    public class ConfigurationDetailTests
    {
        [Fact]
        public void RunSimpleAsync_Emits_Config_Loading()
        {
            var source = @"
    namespace Prova.Generators.Tests
    {
    using Prova;
    
    public class MyTests
    {
        [Fact]
        public void Test1() { }
    }";
            var expected = "var config = global::Prova.Configuration.ConfigLoader.Load();";
            GeneratorVerifier.VerifyContains(source, expected);
        }

        [Fact]
        public void RunSimpleAsync_Emits_Effective_Retry_Logic()
        {
            var source = @"
    namespace Prova.Generators.Tests
    {
    using Prova;
    
    public class MyTests
    {
        [Fact]
        public void Test1() { }
    }";
            var expected = "var effectiveRetry = test.RetryCount ?? config.DefaultRetryCount ?? 0;";
            GeneratorVerifier.VerifyContains(source, expected);
        }

        [Fact]
        public void RunSimpleAsync_Emits_Effective_Timeout_Logic()
        {
            var source = @"
    namespace Prova.Generators.Tests
    {
    using Prova;
    
    public class MyTests
    {
        [Fact]
        public void Test1() { }
    }";
            var expected = "var effectiveTimeout = test.Timeout ?? config.DefaultTimeoutMs;";
            GeneratorVerifier.VerifyContains(source, expected);
        }

        [Fact]
        public void ProvaTest_Registration_Emits_Nullable_Retry_And_Repeat()
        {
            var source = @"
    namespace Prova.Generators.Tests
    {
    using Prova;
    
    public class MyTests
    {
        [Fact]
        public void Test1() { }
    }";
            // Without attributes, they should be null
            var expectedRetry = "RetryCount = null,";
            var expectedRepeat = "Repeat = null,";
            
            GeneratorVerifier.VerifyContains(source, expectedRetry);
            GeneratorVerifier.VerifyContains(source, expectedRepeat);
        }

        [Fact]
        public void ProvaTest_Registration_Emits_Constant_When_Attribute_Present()
        {
            var source = @"
    namespace Prova.Generators.Tests
    {
    using Prova;
    
    public class MyTests
    {
        [Fact]
        [Retry(3)]
        [Repeat(2)]
        public void Test1() { }
    }";
            var expectedRetry = "RetryCount = 3,";
            var expectedRepeat = "Repeat = 2,";
            
            GeneratorVerifier.VerifyContains(source, expectedRetry);
            GeneratorVerifier.VerifyContains(source, expectedRepeat);
        }
    }
}
}
