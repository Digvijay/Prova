using System;
using Prova;

namespace Prova.Demo
{
    public interface IService
    {
        string GetMessage();
    }

    public class MyService : IService
    {
        public string GetMessage() => "Hello from AOT DI! 💉";
    }

    public static class TestDependencies
    {
        [TestDependency]
        public static IService CreateService()
        {
            return new MyService();
        }
    }

    public class DependencyTests
    {
        private readonly IService _service;

        public DependencyTests(IService service)
        {
            _service = service;
        }

        [Fact]
        public void InjectionWorks()
        {
            Assert.Equal("Hello from AOT DI! 💉", _service.GetMessage());
        }
    }
}
