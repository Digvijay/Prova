using System.Threading.Tasks;
using Prova;

namespace Prova.Demo
{
    /// <summary>
    /// Tests for validating output capture.
    /// </summary>
    public class OutputCaptureTests
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCaptureTests"/> class.
        /// </summary>
        public OutputCaptureTests(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// Logs some output.
        /// </summary>
        [Fact]
        public void LogSomething()
        {
            _output.WriteLine("Hello from Prova! 📝");
            _output.WriteLine("This is a captured log.");
        }
    }
}
