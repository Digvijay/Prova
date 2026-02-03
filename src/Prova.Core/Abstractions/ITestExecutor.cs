using System;
using System.Threading.Tasks;

namespace Prova
{
    /// <summary>
    /// Allows custom control over how a test method is invoked.
    /// </summary>
    public interface ITestExecutor
    {
        /// <summary>
        /// Executes the test action.
        /// </summary>
        /// <param name="testAction">The delegate representing the test execution.</param>
        /// <param name="testInfo">Metadata about the test being executed.</param>
        /// <returns>The test output, if any.</returns>
        Task<string?> ExecuteAsync(Func<Task<string?>> testAction, ProvaTest testInfo);
    }
}
