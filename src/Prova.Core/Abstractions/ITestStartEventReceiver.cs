using System.Threading.Tasks;

namespace Prova
{
    /// <summary>
    /// Receives events when a test execution starts.
    /// </summary>
    public interface ITestStartEventReceiver : ITestEventReceiver
    {
        /// <summary>
        /// Called when a test is about to start.
        /// </summary>
        /// <param name="test">The test metadata.</param>
        Task OnTestStartAsync(ProvaTest test);
    }
}
