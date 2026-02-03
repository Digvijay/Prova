using System;

namespace Prova
{
    /// <summary>
    /// Specifies that the method should be executed once after all tests in the class.
    /// The method must be static.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AfterClassAttribute : AfterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterClassAttribute"/> class.
        /// </summary>
        public AfterClassAttribute() : base(HookScope.Class) { }
    }

    /// <summary>
    /// Alias for [AfterClass].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AfterAllAttribute : AfterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterAllAttribute"/> class.
        /// </summary>
        public AfterAllAttribute() : base(HookScope.Class) { }
    }
}
