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
        /// <summary>Gets the display name of the test.</summary>
        public required string DisplayName { get; set; }

        /// <summary>Gets the full hierarchical name of the test (Namespace.Class.Method).</summary>
        public string? FullName { get; set; }

        /// <summary>Gets the name of the class containing the test.</summary>
        public string? ClassName { get; set; }

        /// <summary>Gets the description of the test extracted from XML comments.</summary>
        public string? Description { get; set; }

        /// <summary>Gets the reason why the test should be skipped, or null if it should run.</summary>
        public string? SkipReason { get; set; }

        /// <summary>Gets the key-value pairs representing the test properties and traits.</summary>
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        /// <summary>Gets the number of times to retry the test if it fails.</summary>
        public int? RetryCount { get; set; }

        /// <summary>Gets the maximum number of concurrent tests to run for the class or project.</summary>
        public int? MaxParallel { get; set; }

        /// <summary>Gets a value indicating whether the test should run in isolation.</summary>
        public bool DoNotParallelize { get; set; }

        /// <summary>Gets the unique ID used for code coverage tracking.</summary>
        public int CoverageId { get; set; } = -1;

        /// <summary>Gets the delegate used to execute the test logic. Returns captured output if any.</summary>
        public required Func<Task<string?>> ExecuteDelegate { get; set; }

        /// <summary>Hooks to run once before all tests in the class.</summary>
        public IEnumerable<Func<Task>>? ClassBefore { get; set; }

        /// <summary>Hooks to run once after all tests in the class.</summary>
        public IEnumerable<Func<Task>>? ClassAfter { get; set; }

        /// <summary>Fixtures that should be initialized for this class.</summary>
        public IEnumerable<Func<Task>>? ClassFixtures { get; set; }

        /// <summary>Gets the list of resource keys that this test needs exclusive access to.</summary>
        public IList<string>? ResourceConstraints { get; set; }

        /// <summary>Gets the number of times to repeat the test.</summary>
        public int? Repeat { get; set; }

        /// <summary>Gets the timeout in milliseconds for the test execution.</summary>
        public int? Timeout { get; set; }

        /// <summary>Gets the specific culture to run the test under.</summary>
        public string? Culture { get; set; }

        /// <summary>Gets the name of the parallel group the test belongs to.</summary>
        public string? ParallelGroup { get; set; }

        /// <summary>Gets the concurrency limiters for this test.</summary>
        public IList<(string Key, int Limit)>? ParallelLimiters { get; set; }

        /// <summary>Gets the type of the custom executor to use, if any.</summary>
        public global::System.Type? ExecutorType { get; set; }

        /// <summary>Gets the name of the test variant, if any.</summary>
        public string? Variant { get; set; }
    }
}
