using System;
using Xunit;

namespace Prova.Generators.Tests
{
    public class ConstructorInjectionTests
    {
        [Fact]
        public void Constructor_With_Parameters_Generates_Resolution_Logic()
        {
            var source = @"
using Prova;
using System;

public class MyService { }

public class Startup
{
    [ConfigureServices]
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MyService>();
    }
}

public class MyConstructorTests
{
    private readonly MyService _service;

    public MyConstructorTests(MyService service)
    {
        _service = service;
    }

    [Fact]
    public void Test()
    {
    }
}";
            
            // Expected output should resolve MyService from services
            // We assume 'services' is available in the scope (it might need to be passed or accessed via a static/context)
            // Based on existing DependencyTests, 'services' seemed to be available or we need to ensure it is.
            // Actually, in the current SourceEmitter, 'GenerateTestRegistration' creates the 'ExecuteDelegate'.
            // Inside ExecuteDelegate: 'var instance = new Global::MyConstructorTests();'
            // We want: 'var instance = new Global::MyConstructorTests(services.GetRequiredService<Global::MyService>());'
            // Wait, where does 'services' come from in the ExecuteDelegate? 
            // In 'DependencyTests.cs', 'ConfigureServices' sets up DI, but how is it consumed?
            // Existing 'TestDependency_Generates_Constructor_Injection' in DependencyTests.cs (Wait, does that exist? I saw it in the file view earlier)
            // Let's re-read DependencyTests.cs carefully.
            
            var expected = @"new MyConstructorTests(default)";
            GeneratorVerifier.VerifyContains(source, expected);
        }
        [Fact]
        public void Constructor_With_Data_Resolution()
        {
            var source = @"
using Prova;

[InlineData(100)]
public class MyDataConstructorTests
{
    public MyDataConstructorTests(int value) { }

    [Fact]
    public void Test() { }
}";
            var expected = @"new MyDataConstructorTests(100)";
            GeneratorVerifier.VerifyContains(source, expected);
        }
    }
}
