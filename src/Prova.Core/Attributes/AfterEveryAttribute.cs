using System;
using Prova;

namespace Prova
{
    /// <summary>
    /// Marks a static method to be invoked after every test or class.
    /// Unlike [After], this runs for all tests/classes, not just the containing class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AfterEveryAttribute : Attribute
    {
        /// <summary>Gets the scope of the hook.</summary>
        public HookScope Scope { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterEveryAttribute"/> class.
        /// </summary>
        /// <param name="scope">The scope of the hook.</param>
        public AfterEveryAttribute(HookScope scope = HookScope.Test)
        {
            Scope = scope;
        }
    }
}
