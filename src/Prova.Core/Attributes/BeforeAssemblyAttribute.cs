using System;

namespace Prova
{
    /// <summary>
    /// Specifies that the method should be executed once before any tests in the assembly.
    /// The method must be static.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class BeforeAssemblyAttribute : BeforeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeAssemblyAttribute"/> class.
        /// </summary>
        public BeforeAssemblyAttribute() : base(HookScope.Assembly) { }
    }
}
