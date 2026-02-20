using System;

namespace Prova
{
    /// <summary>
    /// Sets the <see cref="System.Globalization.CultureInfo.CurrentCulture"/> and <see cref="System.Globalization.CultureInfo.CurrentUICulture"/> for the test execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CultureAttribute : Attribute
    {
        /// <summary>Gets the culture name (e.g., "en-US").</summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureAttribute"/> class.
        /// </summary>
        /// <param name="name">The culture name.</param>
        public CultureAttribute(string name)
        {
            Name = name;
        }
    }
}
