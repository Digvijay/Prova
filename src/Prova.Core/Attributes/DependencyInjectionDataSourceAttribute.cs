using System;

namespace Prova
{
    /// <summary>
    /// Sourced data from a type registered in the Dependency Injection container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class DependencyInjectionDataSourceAttribute : Attribute
    {
        /// <summary>Gets the type of the provider.</summary>
        public Type ProviderType { get; }

        /// <summary>Gets the name of the member that provides the data.</summary>
        public string? MemberName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionDataSourceAttribute"/> class.
        /// </summary>
        /// <param name="providerType">The type of the provider.</param>
        /// <param name="memberName">The name of the member.</param>
        public DependencyInjectionDataSourceAttribute(Type providerType, string? memberName = null)
        {
            ProviderType = providerType;
            MemberName = memberName;
        }
    }
}
