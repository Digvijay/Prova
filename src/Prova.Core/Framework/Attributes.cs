namespace Prova
{
    /// <summary>
    /// Specifies that a test should be retried a given number of times if it fails.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RetryAttribute : Attribute
    {
        /// <summary>Gets the number of times to retry the test.</summary>
        public int Count { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryAttribute"/> class.
        /// </summary>
        /// <param name="count">The number of times to retry.</param>
        public RetryAttribute(int count)
        {
            Count = count;
        }
    }

    /// <summary>
    /// Specifies that only tests marked with this attribute should be run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FocusAttribute : Attribute
    {
    }
}
