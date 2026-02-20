using System;
using Prova;

namespace Prova
{
    /// <summary>
    /// Marks a static method to be invoked before every test or class.
    /// Unlike [Before], this runs for all tests/classes, not just the containing class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class BeforeEveryAttribute : Attribute
    {
        /// <summary>Gets the scope of the hook.</summary>
        public HookScope Scope { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeEveryAttribute"/> class.
        /// </summary>
        /// <param name="scope">The scope of the hook.</param>
        public BeforeEveryAttribute(HookScope scope = HookScope.Test)
        {
            Scope = scope;
        }
    }
}
