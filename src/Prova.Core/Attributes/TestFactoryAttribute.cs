using System;

namespace Prova
{
    /// <summary>
    /// Marks a static method as a factory that generates dynamic tests at runtime.
    /// The method must accept a <see cref="DynamicTestBuilder"/> as its first parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestFactoryAttribute : Attribute
    {
    }
}
