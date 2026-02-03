using System;

namespace Prova
{
    /// <summary>
    /// Indicates that the test or class uses shared resources and cannot run in parallel with other tests using the same resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class NotInParallelAttribute : Attribute
    {
        /// <summary>
        /// Gets the list of resource keys that this test needs exclusive access to.
        /// </summary>
        public string[] ResourceKeys { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotInParallelAttribute"/> class.
        /// </summary>
        /// <param name="resourceKeys">The keys of the resources (e.g. "Database", "FileSystem").</param>
        public NotInParallelAttribute(params string[] resourceKeys)
        {
            ResourceKeys = resourceKeys;
        }
    }
}
