using System;

namespace Prova
{
    /// <summary>
    /// Specifies a timeout for a test in milliseconds.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class TimeoutAttribute : Attribute
    {
        /// <summary>Gets the timeout in milliseconds.</summary>
        public int Milliseconds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutAttribute"/> class.
        /// </summary>
        /// <param name="milliseconds">The timeout in milliseconds.</param>
        public TimeoutAttribute(int milliseconds)
        {
            Milliseconds = milliseconds;
        }
    }
}
