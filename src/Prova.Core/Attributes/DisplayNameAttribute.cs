using System;

namespace Prova
{
    /// <summary>
    /// Specifies a custom display name for the test method.
    /// You can use placeholders like {0}, {1} to include argument values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class DisplayNameAttribute : Attribute
    {
        /// <summary>Gets the format string for the display name.</summary>
        public string Format { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="format">The format string.</param>
        public DisplayNameAttribute(string format)
        {
            Format = format;
        }
    }
}
