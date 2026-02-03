using System;

namespace Prova
{
    /// <summary>
    /// Specifies that the method should be executed once before any tests in the class.
    /// The method must be static.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class BeforeClassAttribute : BeforeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeClassAttribute"/> class.
        /// </summary>
        public BeforeClassAttribute() : base(HookScope.Class) { }
    }

    /// <summary>
    /// Alias for [BeforeClass].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class BeforeAllAttribute : BeforeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeAllAttribute"/> class.
        /// </summary>
        public BeforeAllAttribute() : base(HookScope.Class) { }
    }
}
