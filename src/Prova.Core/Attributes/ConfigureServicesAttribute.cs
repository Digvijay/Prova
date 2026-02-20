using System;

namespace Prova
{
    /// <summary>
    /// Marks a method to be used for configuring services in the Dependency Injection container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ConfigureServicesAttribute : Attribute
    {
    }
}
