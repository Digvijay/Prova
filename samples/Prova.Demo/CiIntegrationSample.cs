using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class CiIntegrationSample
    {
        [Fact]
        public void LogWarning_Generates_CI_Output()
        {
            TestContext.Current.Output.WriteLine("Testing CI Warning...");
            TestContext.Current.Logger.LogWarning("This is a CI Warning");
        }

        [Fact]
        public void LogError_Generates_CI_Output()
        {
            TestContext.Current.Output.WriteLine("Testing CI Error...");
            TestContext.Current.Logger.LogError("This is a CI Error");
        }
    }
}
