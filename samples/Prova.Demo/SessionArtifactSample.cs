using Prova;
using System.Threading.Tasks;
using System.IO;

namespace Prova.Demo
{
    public class SessionArtifactSample
    {
        [BeforeAssembly]
        public static async Task Before()
        {
            await Task.Yield();
            // This should use the Session Context
            // TestContext.Current.Properties["SessionStart"] = System.DateTime.Now.ToString();
            TestContext.Current.Output.WriteLine("Session Starting...");
            TestContext.Current.Output.AttachArtifact("session-start.log", "Session Start Log", "text/plain");
        }

        [AfterAssembly]
        public static async Task After()
        {
            await Task.Yield();
            // This should use the Session Context
            TestContext.Current.Output.WriteLine("Session Ending...");
            TestContext.Current.Output.AttachArtifact("session-end.log", "Session End Log", "text/plain");
        }

        [Fact]
        public void TestInSession()
        {
            // This runs in its own Test Context, but should not affect the Session Context restoration
            TestContext.Current.Output.WriteLine("Test Running...");
        }
    }
}
