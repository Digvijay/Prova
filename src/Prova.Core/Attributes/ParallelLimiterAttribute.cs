using System;

namespace Prova
{
    /// <summary>
    /// Limits the concurrency of tests that share a specific resource key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class ParallelLimiterAttribute : Attribute
    {
        /// <summary>Gets the key of the resource to limit.</summary>
        public string Key { get; }

        /// <summary>Gets the maximum number of concurrent tests for this key.</summary>
        public int Limit { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelLimiterAttribute"/> class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="limit">The concurrency limit.</param>
        public ParallelLimiterAttribute(string key, int limit)
        {
            Key = key;
            Limit = limit;
        }
    }
}
