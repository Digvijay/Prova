namespace Prova
{
    /// <summary>
    /// Exception thrown when a test assertion fails.
    /// </summary>
    public class AssertException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AssertException(string message) : base(message)
        {
        }
    }
}
