using System;
using Prova;

namespace Prova
{
    /// <summary>
    /// Attribute to mark a method to be executed after a test hook scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AfterAttribute : Attribute
    {
        /// <summary>
        /// Gets the scope of the hook.
        /// </summary>
        public HookScope Scope { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterAttribute"/> class.
        /// </summary>
        /// <param name="scope">The scope of the hook.</param>
        public AfterAttribute(HookScope scope = HookScope.Test)
        {
            Scope = scope;
        }
    }

    /// <summary>
    /// Alias for [After].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AfterEachAttribute : AfterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterEachAttribute"/> class.
        /// </summary>
        public AfterEachAttribute() : base(HookScope.Test) { }
    }
}
