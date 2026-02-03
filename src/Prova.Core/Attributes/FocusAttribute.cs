using System;

namespace Prova
{
    /// <summary>
    /// Specifies that only tests marked with this attribute should be run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FocusAttribute : Attribute
    {
    }
}
