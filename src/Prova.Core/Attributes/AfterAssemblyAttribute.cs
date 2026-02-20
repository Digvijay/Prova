using System;

namespace Prova
{
    /// <summary>
    /// Specifies that the method should be executed once after all tests in the assembly.
    /// The method must be static.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AfterAssemblyAttribute : AfterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterAssemblyAttribute"/> class.
        /// </summary>
        public AfterAssemblyAttribute() : base(HookScope.Assembly) { }
    }
}
