using System;
using Prova;
using Prova.Core;

namespace Prova.Demo
{
    [ClassFactory(typeof(MySampleFactory))]
    public class ClassFactorySample
    {
        private readonly string _message;

        public ClassFactorySample(string message)
        {
            _message = message;
        }

        [Fact]
        public void Test_FactoryInjection()
        {
            Console.WriteLine($"Message in test: {_message}");
            Assert.Equal("Injected via Factory", _message);
        }
    }

    public class MySampleFactory : IClassConstructor<ClassFactorySample>
    {
        public ClassFactorySample CreateInstance(IServiceProvider services)
        {
            Console.WriteLine("🏭 MySampleFactory: Instinctively creating instance...");
            return new ClassFactorySample("Injected via Factory");
        }
    }

    public class ClassFactoryConfiguration
    {
        [ConfigureServices]
        public static void Configure(ProvaServiceCollection services)
        {
            // Register the factory so the runner can resolve it
            services.AddTransient(() => new MySampleFactory());
        }
    }
}
