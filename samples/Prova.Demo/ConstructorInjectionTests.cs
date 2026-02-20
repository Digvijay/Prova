using System;
using System.Threading.Tasks;
using Prova;
using Prova.Core; // Ensure we use Prova.Core namespace for ProvServiceCollection

namespace Prova.Demo
{
    public interface IInjectedService
    {
        string GetMessage();
    }

    public class InjectedService : IInjectedService
    {
        public string GetMessage() => "Hello from DI";
    }

    public class DemoConfiguration
    {
        [ConfigureServices]
        public static void Configure(ProvaServiceProvider services)
        {
            services.AddSingleton<IInjectedService>(() => new InjectedService());
            services.AddSingleton<DatabaseFixture>(() => new DatabaseFixture());
            services.AddTransient(() => new UserDataProvider());
            services.AddTransient(() => new OrderDataSource());

            // Register Event Receiver
            EventRegistry.Register(new EventSample());
        }
    }

    public class ConstructorInjectionTests
    {
        private readonly IInjectedService _service;

        public ConstructorInjectionTests(IInjectedService service)
        {
            _service = service;
        }

        [Fact]
        public void Can_Inject_Service_Via_Constructor()
        {
            if (_service == null)
                throw new InvalidOperationException("Service was not injected");
            
            var msg = _service.GetMessage();
            Console.WriteLine($"Service says: {msg}");
            
            if (msg != "Hello from DI")
                throw new InvalidOperationException($"Expected 'Hello from DI' but got '{msg}'");
        }
    }
}
