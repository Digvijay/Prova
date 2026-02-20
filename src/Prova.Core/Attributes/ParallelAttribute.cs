using System;

namespace Prova
{
    /// <summary>
    /// Specifies that a test class or method should be executed in parallel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false)]
    public class ParallelAttribute : Attribute
    {
        /// <summary>Gets the maximum degree of parallelism.</summary>
        public int? Max { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelAttribute"/> class.
        /// Defaults to unlimited (processor count).
        /// </summary>
        public ParallelAttribute()
        {
            Max = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelAttribute"/> class.
        /// </summary>
        /// <param name="max">The maximum number of concurrent tests.</param>
        public ParallelAttribute(int max)
        {
            Max = max;
        }
    }
}
