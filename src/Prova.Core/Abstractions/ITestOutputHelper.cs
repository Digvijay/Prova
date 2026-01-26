namespace Prova
{
    /// <summary>
    /// Represents a class used to capture output from a running test.
    /// </summary>
    public interface ITestOutputHelper
    {
        /// <summary>Writes a line of text to the test output.</summary>
        void WriteLine(string message);
        
        /// <summary>Writes a formatted line of text to the test output.</summary>
        void WriteLine(string format, params object[] args);
    }
}
