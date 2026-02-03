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

        /// <summary>
        /// Attaches a file to the test result.
        /// </summary>
        /// <param name="filePath">The absolute path to the file to attach.</param>
        /// <param name="displayName">Optional display name for the attachment.</param>
        /// <param name="mimeType">Optional MIME type of the attachment.</param>
        void AttachArtifact(string filePath, string? displayName = null, string? mimeType = null);

        /// <summary>
        /// Gets the captured output.
        /// </summary>
        string Output { get; }
    }
}
