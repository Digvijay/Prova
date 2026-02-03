using Prova;
using System;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public static class DynamicSample
    {
        [TestFactory]
        public static void RegisterDynamicTests(DynamicTestBuilder builder)
        {
            // Generate tests from a simple loop
            for (int i = 1; i <= 3; i++)
            {
                int index = i;
                builder.Add($"Dynamic.Loop_{index}", () => 
                {
                    Console.WriteLine($"Running dynamic test {index}");
                })
                .WithDescription($"This is dynamic test number {index}")
                .WithProperty("DynamicSource", "Loop");
            }

            // Generate an async test
            builder.Add("Dynamic.AsyncTest", async () =>
            {
                await Task.Delay(10);
                Console.WriteLine("Async dynamic test completed");
            })
            .WithRetry(2)
            .WithTimeout(1000);
            
            // Generate a test that fails
            builder.Add("Dynamic.FailingTest", () =>
            {
                throw new Exception("This dynamic test is designed to fail.");
            })
            .WithSkip("Skipping this failing dynamic test for now")
            .WithDescription("A test that would fail if not skipped");
        }
    }
}
