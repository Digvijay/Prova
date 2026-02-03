using System;
using System.Threading.Tasks;

namespace Prova
{
    /// <summary>
    /// Allows custom control over how a lifecycle hook is invoked.
    /// </summary>
    public interface IHookExecutor
    {
        /// <summary>
        /// Executes the hook action.
        /// </summary>
        /// <param name="hookAction">The delegate representing the hook execution.</param>
        /// <param name="hookName">The name of the hook being executed.</param>
        Task ExecuteAsync(Func<Task> hookAction, string hookName);
    }
}
