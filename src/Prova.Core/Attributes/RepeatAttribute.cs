using System;

namespace Prova
{
    /// <summary>
    /// Specifies that a test method should be executed multiple times.
    /// If any iteration fails, the test is considered failed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RepeatAttribute : Attribute
    {
        /// <summary>Gets the number of times to repeat the test.</summary>
        public int Count { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatAttribute"/> class.
        /// </summary>
        /// <param name="count">The number of times to run the test. Must be greater than 0.</param>
        public RepeatAttribute(int count)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), "Repeat count must be at least 1.");
            Count = count;
        }
    }
}
