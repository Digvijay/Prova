using System;

namespace Prova
{
    /// <summary>
    /// Defines a factory for creating instances of a test class.
    /// </summary>
    /// <typeparam name="T">The type of the test class.</typeparam>
    public interface IClassConstructor<T> where T : class
    {
        /// <summary>
        /// Creates an instance of the test class.
        /// </summary>
        /// <param name="services">The service provider to resolve dependencies.</param>
        /// <returns>A new instance of the test class.</returns>
        T CreateInstance(IServiceProvider services);
    }
}
