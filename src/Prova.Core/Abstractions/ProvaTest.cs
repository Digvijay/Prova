using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prova
{
    /// <summary>
    /// Represents a test metadata and its execution delegate.
    /// </summary>
    public sealed class ProvaTest
    {
        /// <summary>Gets the full name of the test (e.g., Namespace.Class.Method).</summary>
        public required string DisplayName { get; init; }

        /// <summary>Gets the description of the test extracted from XML comments.</summary>
        public string? Description { get; init; }

        /// <summary>Gets the reason why the test should be skipped, or null if it should run.</summary>
        public string? SkipReason { get; init; }

        /// <summary>Gets the key-value pairs representing the test traits.</summary>
        public KeyValuePair<string, string>[] Traits { get; init; } = Array.Empty<KeyValuePair<string, string>>();

        /// <summary>Gets the number of times to retry the test if it fails.</summary>
        public int RetryCount { get; init; }

        /// <summary>Gets the delegate used to execute the test logic.</summary>
        public required Func<Task> ExecuteDelegate { get; init; }
    }
}
