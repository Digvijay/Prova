using System;
using Prova;

namespace Prova
{
    /// <summary>
    /// Attribute to mark a method to be executed before a test hook scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class BeforeAttribute : Attribute
    {
        /// <summary>
        /// Gets the scope of the hook.
        /// </summary>
        public HookScope Scope { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeAttribute"/> class.
        /// </summary>
        /// <param name="scope">The scope of the hook.</param>
        public BeforeAttribute(HookScope scope = HookScope.Test)
        {
            Scope = scope;
        }
    }

    /// <summary>
    /// Alias for [Before].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class BeforeEachAttribute : BeforeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeEachAttribute"/> class.
        /// </summary>
        public BeforeEachAttribute() : base(HookScope.Test) { }
    }
}
