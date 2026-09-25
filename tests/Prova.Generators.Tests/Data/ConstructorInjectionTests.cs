using System;

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

            // Constructor parameters are resolved from the service container.
            GeneratorVerifier.VerifyContains(source, "new MyConstructorTests(TestRunnerExecutor.Services.Get<MyService>())");
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
