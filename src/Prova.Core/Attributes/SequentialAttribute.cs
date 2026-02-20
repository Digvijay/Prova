using System;

namespace Prova
{
    /// <summary>
    /// Specifies that tests in this class or this specific test should run serially.
    /// Alias for [Parallel(1)].
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false)]
    public class SequentialAttribute : ParallelAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialAttribute"/> class.
        /// </summary>
        public SequentialAttribute() : base(1) { }
    }
}
