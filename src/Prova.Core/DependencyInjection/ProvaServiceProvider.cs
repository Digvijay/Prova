using System;
using System.Collections.Generic;

namespace Prova.Core
{
    /// <summary>
    /// A lightweight, zero-reflection service provider for Prova tests.
    /// </summary>
    public class ProvaServiceProvider : IServiceProvider, IDisposable
    {
        private readonly Dictionary<Type, Func<object>> _transients = new Dictionary<Type, Func<object>>();
        private readonly Dictionary<Type, Lazy<object>> _singletons = new Dictionary<Type, Lazy<object>>();

        /// <summary>
        /// Registers a transient service. A new instance is created every time it is requested.
        /// </summary>
        public void AddTransient<T>(Func<T> factory) where T : class
        {
            _transients[typeof(T)] = factory;
        }

        /// <summary>
        /// Registers a singleton service. The instance is created lazily on first request and reused.
        /// </summary>
        public void AddSingleton<T>(Func<T> factory) where T : class
        {
            _singletons[typeof(T)] = new Lazy<object>(() => factory());
        }

        /// <summary>
        /// Resolves a service of type T. Throws if not found.
        /// </summary>
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            
            if (_singletons.TryGetValue(type, out var lazy))
            {
                var instance = (T)lazy.Value;
                return instance;
            }
            
            if (_transients.TryGetValue(type, out var factory))
            {
                return (T)factory();
            }
            
            throw new InvalidOperationException($"No service registered for type '{type.FullName}'. Ensure it is registered in your [ConfigureServices] method.");
        }

        /// <summary>
        /// Returns this instance as a simple service provider.
        /// </summary>
        public ProvaServiceProvider BuildServiceProvider() => this;

        /// <summary>
        /// Resolves a service of type type. Returns null if not found.
        /// </summary>
        public object? GetService(Type serviceType)
        {
            if (_singletons.TryGetValue(serviceType, out var lazy))
            {
                return lazy.Value;
            }
            
            if (_transients.TryGetValue(serviceType, out var factory))
            {
                return factory();
            }

            return null;
        }

        /// <summary>
        /// Disposes all singleton services that implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            foreach (var lazy in _singletons.Values)
            {
                if (lazy.IsValueCreated && lazy.Value is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch
                    {
                        // Ignore disposal errors
                    }
                }
            }
            GC.SuppressFinalize(this);
        }
    }
}
