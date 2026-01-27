using System;
using Prova;

namespace Prova.Core.Tests.Framework
{
    /// <summary>
    /// Tests retry and flaky behavior.
    /// </summary>
    public class FlakyTests
    {
        private static int _attempts;

        /// <summary>
        /// A flaky test that passes on the 3rd attempt.
        /// </summary>
        [Fact]
        [Retry(3)]
        // [Focus]
        public static void FlakyPassTest()
        {
            _attempts++;
            if (_attempts < 3)
            {
                throw new InvalidOperationException($"Failed on attempt {_attempts}");
            }
            // Passes on 3rd attempt
            Assert.True(true);
        }

        /// <summary>
        /// A normal passing test.
        /// </summary>
        [Fact]
        public static void NormalTest()
        {
             Assert.True(true);
        }

        /*
        [Fact]
        [Focus]
        public void FocusedTest()
        {
             Assert.True(true);
        }
        */
        // Uncommenting [Focus] above should make only FocusedTest run.
        // We will test Focus separately to avoid skipping everything in the main run.
    }
}
