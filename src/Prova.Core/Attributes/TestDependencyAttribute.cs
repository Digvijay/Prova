using System;

namespace Prova
{
    /// <summary>
    /// Marks a static method as a factory for a dependency type.
    /// The method must be static, public, and return the specified type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestDependencyAttribute : Attribute
    {
    }
}
