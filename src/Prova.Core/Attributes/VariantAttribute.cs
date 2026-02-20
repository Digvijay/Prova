using System;

namespace Prova
{
    /// <summary>
    /// Specifies type arguments for a generic test class or method.
    /// Used for compile-time expansion to ensure AOT safety.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class VariantAttribute : Attribute
    {
        /// <summary>Gets the generic type arguments.</summary>
        public Type[] Types { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariantAttribute"/> class.
        /// </summary>
        /// <param name="types">The concrete types to use for the generic parameters.</param>
        public VariantAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
