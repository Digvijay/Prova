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

    /// <summary>
    /// Provides a description for a test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>Gets the description text.</summary>
        public string Text { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionAttribute"/> class.
        /// </summary>
        /// <param name="text">The description of the test.</param>
        public DescriptionAttribute(string text)
        {
            Text = text;
        }
    }

    /// <summary>
    /// Specifies the maximum number of concurrent tests to run for a class or project.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ParallelAttribute : Attribute
    {
        /// <summary>Gets the maximum degree of parallelism.</summary>
        public int Max { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelAttribute"/> class.
        /// </summary>
        /// <param name="max">The maximum number of concurrent tests.</param>
        public ParallelAttribute(int max)
        {
            Max = max;
        }
    }

    /// <summary>
    /// Specifies that the test data comes from a class that implements IEnumerable&lt;object[]&gt;.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ClassDataAttribute : Attribute
    {
        /// <summary>Gets the type of the class that provides the test data.</summary>
        public Type Class { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDataAttribute"/> class.
        /// </summary>
        /// <param name="classType">The type of the class that provides the data.</param>
        public ClassDataAttribute(Type classType)
        {
            Class = classType;
        }
    }

    /// <summary>
    /// Marks a static method as a factory for a dependency type.
    /// The method must be static, public, and return the specified type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestDependencyAttribute : Attribute
    {
        
    }

    /// <summary>
    /// Specifies the maximum number of bytes that a test is allowed to allocate on the heap.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MaxAllocAttribute : Attribute
    {
        /// <summary>Gets the maximum allowed allocation in bytes.</summary>
        public long Bytes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxAllocAttribute"/> class.
        /// </summary>
        /// <param name="bytes">The allocation limit in bytes.</param>
        public MaxAllocAttribute(long bytes)
        {
            Bytes = bytes;
        }
    }

    /// <summary>
    /// Specifies a timeout for a test in milliseconds.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TimeoutAttribute : Attribute
    {
        /// <summary>Gets the timeout in milliseconds.</summary>
        public int Milliseconds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutAttribute"/> class.
        /// </summary>
        /// <param name="milliseconds">The timeout in milliseconds.</param>
        public TimeoutAttribute(int milliseconds)
        {
            Milliseconds = milliseconds;
        }
    }
}
