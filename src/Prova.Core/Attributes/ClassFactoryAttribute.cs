using System;

namespace Prova
{
    /// <summary>
    /// Specifies a custom factory for instantiating the test class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ClassFactoryAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of the factory that implements IClassConstructor.
        /// </summary>
        public Type FactoryType { get; }

        /// <summary>
        /// Initializes a new instance of the ClassFactoryAttribute.
        /// </summary>
        /// <param name="factoryType">The type of the factory. It must implement IClassConstructor&lt;T&gt; where T is the test class.</param>
        public ClassFactoryAttribute(Type factoryType)
        {
            FactoryType = factoryType;
        }
    }
}
