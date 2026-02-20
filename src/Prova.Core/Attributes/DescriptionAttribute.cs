using System;

namespace Prova
{
    /// <summary>
    /// Provides a description for a test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>Gets the description text.</summary>
        public string Text { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionAttribute"/> class.
        /// </summary>
        /// <param name="text">The description of the test.</param>
        public DescriptionAttribute(string text)
        {
            Text = text;
        }
    }
}
