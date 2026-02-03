namespace Prova
{
    /// <summary>
    /// Default implementation of <see cref="ITestOutputHelper"/>.
    /// </summary>
    public class TestOutputHelper : ITestOutputHelper
    {
        private readonly System.Text.StringBuilder _buffer = new();

        /// <inheritdoc />
        public void WriteLine(string message)
        {
            _buffer.AppendLine(message);
        }

        /// <inheritdoc />
        public void WriteLine(string format, params object[] args)
        {
            _buffer.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>Gets the captured output.</summary>
        public string Output => _buffer.ToString();

        /// <inheritdoc />
        public void AttachArtifact(string filePath, string? displayName = null, string? mimeType = null)
        {
            var name = displayName ?? System.IO.Path.GetFileName(filePath);
            // Using a structured format that can be parsed by reporters
            _buffer.AppendLine(string.Format(global::System.Globalization.CultureInfo.InvariantCulture, "[[ATTACHMENT|{0}|{1}|{2}]]", filePath, name, mimeType));
        }
    }
}
