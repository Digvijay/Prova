namespace Prova.Reporters
{
    /// <summary>
    /// Defines a reporter for test execution events.
    /// </summary>
    public interface ITestReporter
    {
        /// <summary>Called when a test starts.</summary>
        void OnTestStarting(string testName, string? description = null);
        
        /// <summary>Called when a test succeeds.</summary>
        void OnTestSuccess(string testName, string output);
        
        /// <summary>Called when a test fails.</summary>
        void OnTestFailure(string testName, Exception ex, string output);
        
        /// <summary>Called when a test is skipped.</summary>
        void OnTestSkipped(string testName, string reason);
        
        /// <summary>Called when all tests have completed.</summary>
        void OnComplete(int passed, int failed, int skipped, TimeSpan duration);
    }
}
