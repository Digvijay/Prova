using System;

namespace Prova
{
    /// <summary>
    /// Specifies a custom formatter for a test parameter.
    /// The formatter type must implement <see cref="IArgumentFormatter"/> and have a parameterless constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class ArgumentDisplayFormatterAttribute : Attribute
    {
        /// <summary>Gets the type of the formatter.</summary>
        public Type FormatterType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentDisplayFormatterAttribute"/> class.
        /// </summary>
        /// <param name="formatterType">The type of the formatter.</param>
        public ArgumentDisplayFormatterAttribute(Type formatterType)
        {
            FormatterType = formatterType;
        }
    }
}
