using System;

namespace Prova
{
    /// <summary>
    /// Adds arbitrary metadata to a test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PropertyAttribute : Attribute
    {
        /// <summary>Gets the key of the property.</summary>
        public string Key { get; }

        /// <summary>Gets the value of the property.</summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
        /// </summary>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The value of the property.</param>
        public PropertyAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
