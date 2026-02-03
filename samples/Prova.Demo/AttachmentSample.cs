using System.Threading.Tasks;
using Prova;

namespace Prova.Demo
{
    public class AttachmentSample
    {
        [Fact]
        public void AttachLogs()
        {
            TestContext.Current.Output.WriteLine("Generating log file...");
            TestContext.Current.Output.AttachArtifact("/var/logs/app.log", "Application Log", "text/plain");
        }

        [Fact]
        public void AttachImage()
        {
            TestContext.Current.Output.WriteLine("Taking screenshot...");
            TestContext.Current.Output.AttachArtifact("/var/screenshots/failure.png", "Failure Screenshot", "image/png");
        }
    }
}
