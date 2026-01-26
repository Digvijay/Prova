namespace Prova
{
    /// <summary>
    /// Attribute that provides data for a <see cref="TheoryAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class InlineDataAttribute : Attribute
    {
        /// <summary>Gets the data for the test.</summary>
        public object?[] Data { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineDataAttribute"/> class.
        /// </summary>
        /// <param name="data">The data to pass to the test method.</param>
        public InlineDataAttribute(params object?[] data)
        {
            Data = data;
        }
    }
}
