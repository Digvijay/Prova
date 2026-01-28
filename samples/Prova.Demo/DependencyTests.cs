using System;
using Prova;

namespace Prova.Demo
{
    /// <summary>
    /// A simple service interface.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        string GetMessage();
    }

    /// <summary>
    /// A simple service implementation.
    /// </summary>
    public class MyService : IService
    {
        /// <inheritdoc />
        public string GetMessage() => "Hello from AOT DI! 💉";
    }

    /// <summary>
    /// Factory methods for dependencies.
    /// </summary>
    public static class TestDependencies
    {
        /// <summary>
        /// Creates an IService instance.
        /// </summary>
        [TestDependency]
        public static IService CreateService()
        {
            return new MyService();
        }
    }

    /// <summary>
    /// Tests for validating dependency injection.
    /// </summary>
    public class DependencyTests
    {
        private readonly IService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyTests"/> class.
        /// </summary>
        public DependencyTests(IService service)
        {
            _service = service;
        }

        /// <summary>
        /// Verifies that the service was injected.
        /// </summary>
        [Fact]
        public void InjectionWorks()
        {
            Assert.Equal("Hello from AOT DI! 💉", _service.GetMessage());
        }
    }
}
