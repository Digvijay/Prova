using System;

namespace Prova
{
    /// <summary>
    /// Specifies a custom executor to use for a test method, class, or assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public class ExecutorAttribute : Attribute
    {
        /// <summary>
        /// The type of the executor to use. Must implement ITestExecutor or IHookExecutor.
        /// </summary>
        public Type ExecutorType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutorAttribute"/> class.
        /// </summary>
        /// <param name="executorType">The type of the executor to use.</param>
        public ExecutorAttribute(Type executorType)
        {
            ExecutorType = executorType;
        }
    }
}
