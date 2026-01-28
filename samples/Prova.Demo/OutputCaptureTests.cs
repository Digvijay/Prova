using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class OutputCaptureTests
    {
        private readonly ITestOutputHelper _output;

        public OutputCaptureTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void LogSomething()
        {
            _output.WriteLine("Hello from Prova! 📝");
            _output.WriteLine("This is a captured log.");
        }
    }
}
