using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Prova.Logging;

namespace Prova
{
    /// <summary>
    /// Provides information about and control over the currently executing test.
    /// </summary>
    public sealed class TestContext
    {
        private static readonly AsyncLocal<TestContext?> _current = new();

        /// <summary>
        /// Gets the global state bag shared across the entire test run.
        /// </summary>
        public static StateBag GlobalState { get; } = new StateBag();

#pragma warning disable PRV019
        /// <summary>
        /// Gets the <see cref="TestContext"/> for the current execution flow.
        /// </summary>
        public static TestContext Current
        {
            get => _current.Value ?? throw new InvalidOperationException("TestContext.Current is only available during test execution.");
            set => _current.Value = value;
        }
#pragma warning restore PRV019

        /// <summary>
        /// Gets the display name of the current test.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the metadata properties associated with the current test.
        /// </summary>
        public IReadOnlyDictionary<string, string> Properties { get; }

        /// <summary>
        /// Gets a cancellation token that is triggered when the test is cancelled or times out.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets a collection of user-defined items for the current test.
        /// </summary>
        public IDictionary<string, object?> Items { get; } = new ConcurrentDictionary<string, object?>();

        /// <summary>
        /// Gets the name of the current test variant, if any.
        /// </summary>
        public string? Variant { get; }

        /// <summary>
        /// Gets the logger for the current test.
        /// </summary>
        public ITestLogger Logger { get; }

        /// <summary>
        /// Gets the output helper for the current test.
        /// </summary>
        public ITestOutputHelper Output { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestContext"/> class.
        /// </summary>
        /// <param name="displayName">The display name of the test.</param>
        /// <param name="properties">The metadata properties.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="variant">The variant name, if any.</param>
        /// <param name="logger">The logger to use.</param>
        /// <param name="output">The output helper to use.</param>
        public TestContext(string displayName, IDictionary<string, string> properties, CancellationToken cancellationToken, string? variant = null, ITestLogger? logger = null, ITestOutputHelper? output = null)
        {
            DisplayName = displayName;
            Properties = new ReadOnlyDictionary<string, string>(properties);
            CancellationToken = cancellationToken;
            Variant = variant;
            Logger = logger ?? new ConsoleLogger();
            Output = output ?? new TestOutputHelper();
        }
    }
}
