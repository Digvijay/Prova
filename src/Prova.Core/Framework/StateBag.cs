using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Prova
{
    /// <summary>
    /// A thread-safe container for sharing state across the entire test run.
    /// </summary>
    public sealed class StateBag
    {
        private readonly ConcurrentDictionary<string, object?> _items = new();

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        public object? this[string key]
        {
            get => _items.GetValueOrDefault(key);
            set => _items[key] = value;
        }

        /// <summary>
        /// Adds or updates a value in the state bag.
        /// </summary>
        public void Set<T>(string key, T value)
        {
            _items[key] = value;
        }

        /// <summary>
        /// Retrieves a value from the state bag.
        /// Throws KeyNotFoundException if the key does not exist.
        /// Throws InvalidCastException if the type does not match.
        /// </summary>
        public T Get<T>(string key)
        {
            if (!_items.TryGetValue(key, out var value))
                throw new KeyNotFoundException($"Key '{key}' not found in StateBag.");
            
            return (T)value!;
        }

        /// <summary>
        /// Tries to retrieve a value from the state bag.
        /// </summary>
        public bool TryGet<T>(string key, out T? value)
        {
            if (_items.TryGetValue(key, out var obj))
            {
                if (obj is T t)
                {
                    value = t;
                    return true;
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Clears all state.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }
    }
}
