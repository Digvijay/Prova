using System;

namespace Prova
{
    /// <summary>
    /// format an argument for display in test names.
    /// </summary>
    public interface IArgumentFormatter
    {
        /// <summary>
        /// Formats the specified value into a string representation.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string representation of the value.</returns>
        string Format(object? value);
    }
}
