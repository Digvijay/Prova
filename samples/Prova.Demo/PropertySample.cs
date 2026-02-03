using Prova;

namespace Prova.Demo
{
    [Property("Owner", "Digvijay")]
    [Property("Suite", "Smoke")]
    public class PropertySample
    {
        [Fact]
        [Property("Category", "Integration")]
        [Trait("Priority", "High")]
        public void HighPriorityIntegrationTest()
        {
            Console.WriteLine("Running high priority integration test...");
        }

        [Fact]
        [Property("Category", "Unit")]
        public void UnitPropertyTest()
        {
            Console.WriteLine("Running unit property test...");
        }

        [Fact]
        [Property("Owner", "LegacyTeam")] // Override class-level owner
        public void LegacyTest()
        {
            Console.WriteLine("Running legacy test...");
        }
    }
}
