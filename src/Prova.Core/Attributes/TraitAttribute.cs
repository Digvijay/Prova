using System;

namespace Prova
{
    /// <summary>
    /// Alias for <see cref="PropertyAttribute"/> for xUnit compatibility.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TraitAttribute : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraitAttribute"/> class.
        /// </summary>
        /// <param name="key">The key of the trait.</param>
        /// <param name="value">The value of the trait.</param>
        public TraitAttribute(string key, string value) : base(key, value) { }
    }
}
