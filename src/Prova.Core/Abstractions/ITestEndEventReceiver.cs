using System.Threading.Tasks;

namespace Prova
{
    /// <summary>
    /// Specifies the result of a test execution.
    /// </summary>
    public enum TestResult
    {
        /// <summary>The test passed successfully.</summary>
        Passed,
        /// <summary>The test failed.</summary>
        Failed,
        /// <summary>The test was skipped.</summary>
        Skipped
    }

    /// <summary>
    /// Receives events when a test execution completes.
    /// </summary>
    public interface ITestEndEventReceiver : ITestEventReceiver
    {
        /// <summary>
        /// Called when a test has finished executing.
        /// </summary>
        /// <param name="test">The test metadata.</param>
        /// <param name="result">The outcome of the test.</param>
        /// <param name="durationMs">The execution duration in milliseconds.</param>
        Task OnTestEndAsync(ProvaTest test, TestResult result, long durationMs);
    }
}
