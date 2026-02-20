using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prova
{
    /// <summary>
    /// A builder used to imperatively register tests at runtime.
    /// </summary>
    public sealed class DynamicTestBuilder
    {
        private readonly List<ProvaTest> _tests = new();

        /// <summary>
        /// Gets the collection of tests registered so far.
        /// </summary>
        public IEnumerable<ProvaTest> Tests => _tests;

        /// <summary>
        /// Adds a new test to the suite.
        /// </summary>
        /// <param name="name">The display name of the test.</param>
        /// <param name="testLogic">The async delegate to execute.</param>
        /// <returns>A configurator to further refine the test.</returns>
        public TestConfigurator Add(string name, Func<Task> testLogic)
        {
            var test = new ProvaTest
            {
                DisplayName = name,
                ExecuteDelegate = async () => { await testLogic(); return null; }
            };
            _tests.Add(test);
            return new TestConfigurator(test);
        }

        /// <summary>
        /// Adds a synchronous test to the suite.
        /// </summary>
        /// <param name="name">The display name of the test.</param>
        /// <param name="testLogic">The delegate to execute.</param>
        /// <returns>A configurator to further refine the test.</returns>
        public TestConfigurator Add(string name, Action testLogic)
        {
            return Add(name, () => { testLogic(); return Task.CompletedTask; });
        }

        /// <summary>
        /// Adds a pre-configured <see cref="ProvaTest"/> instance.
        /// </summary>
        public void Add(ProvaTest test)
        {
            ArgumentNullException.ThrowIfNull(test);
            _tests.Add(test);
        }

        /// <summary>
        /// Fluent API for configuring a dynamically added test.
        /// </summary>
        public sealed class TestConfigurator
        {
            private readonly ProvaTest _test;

            internal TestConfigurator(ProvaTest test)
            {
                _test = test;
            }

            /// <summary>Sets the description for the test.</summary>
            public TestConfigurator WithDescription(string description)
            {
                _test.Description = description;
                return this;
            }

            /// <summary>Sets the skip reason for the test.</summary>
            public TestConfigurator WithSkip(string reason)
            {
                _test.SkipReason = reason;
                return this;
            }

            /// <summary>Sets the retry count for the test.</summary>
            public TestConfigurator WithRetry(int retryCount)
            {
                _test.RetryCount = retryCount;
                return this;
            }

            /// <summary>Sets the repeat count for the test.</summary>
            public TestConfigurator WithRepeat(int repeatCount)
            {
                _test.Repeat = repeatCount;
                return this;
            }

            /// <summary>Sets the timeout in milliseconds for the test.</summary>
            public TestConfigurator WithTimeout(int timeoutMs)
            {
                _test.Timeout = timeoutMs;
                return this;
            }

            /// <summary>Sets the culture for the test execution.</summary>
            public TestConfigurator WithCulture(string culture)
            {
                _test.Culture = culture;
                return this;
            }

            /// <summary>Groups the test into a logical parallel execution group.</summary>
            public TestConfigurator WithParallelGroup(string groupName)
            {
                _test.ParallelGroup = groupName;
                return this;
            }

            /// <summary>Limits the maximum degree of parallelism for this test.</summary>
            public TestConfigurator WithMaxParallel(int maxParallel)
            {
                _test.MaxParallel = maxParallel;
                return this;
            }

            /// <summary>Prevents the test from running in parallel with other tests.</summary>
            public TestConfigurator DoNotParallelize()
            {
                _test.DoNotParallelize = true;
                return this;
            }

            /// <summary>Adds a custom property to the test.</summary>
            public TestConfigurator WithProperty(string key, string value)
            {
                _test.Properties[key] = value;
                return this;
            }

            /// <summary>Adds a resource constraint to the test.</summary>
            public TestConfigurator WithResourceConstraint(string resourceKey)
            {
                if (_test.ResourceConstraints == null) _test.ResourceConstraints = new List<string>();
                _test.ResourceConstraints.Add(resourceKey);
                return this;
            }
        }
    }
}
