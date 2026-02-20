using System;

namespace Prova
{
    /// <summary>
    /// Specifies that the test or class should not run in parallel with any other tests.
    /// It effectively locks the entire runner for the duration of these tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DoNotParallelizeAttribute : Attribute
    {
    }
}
