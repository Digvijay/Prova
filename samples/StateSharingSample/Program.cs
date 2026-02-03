using Prova;
using System.Threading.Tasks;
using System;

// Script-style entry point
await Prova.TestRunnerExecutor.RunAllAsync(args);

namespace StateSharing
{
    public class Setup
    {
        [BeforeAssembly]
        public static void InitGlobalState()
        {
            Console.WriteLine("🌍 [BeforeAssembly] Setting 'StartTime' and 'Config'...");
            TestContext.GlobalState.Set("StartTime", DateTime.Now);
            TestContext.GlobalState.Set("Config", "Production");
        }
    }

    public class StateTests
    {
        [Fact]
        public void CanAccessGlobalState()
        {
            // Verify we can read what was set in BeforeAssembly
            var config = TestContext.GlobalState.Get<string>("Config");
            Console.WriteLine($"  [Test] Config: {config}");
            Assert.Equal("Production", config);
        }

        [Fact]
        public void CanShareStateBetweenTests()
        {
            // This test is order-dependent if reliant on other tests (anti-pattern usually), 
            // but fine for demonstrating the bag exists.
            
            TestContext.GlobalState.Set("TestRunId", 12345);
            var id = TestContext.GlobalState.Get<int>("TestRunId");
            Assert.Equal(12345, id);
        }

        [Fact]
        public async Task AsyncStateCheck()
        {
            await Task.Delay(10);
            if (TestContext.GlobalState.TryGet<DateTime>("StartTime", out var time))
            {
                Console.WriteLine($"  [Test] Asnyc Checked StartTime: {time}");
                Assert.True(time < DateTime.Now);
            }
            else
            {
                Assert.Fail("StartTime not found in GlobalState");
            }
        }
    }
}
