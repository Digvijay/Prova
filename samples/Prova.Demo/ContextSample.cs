using Prova;
using System;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class ContextSample
    {
        [Fact]
        [Property("Priority", "High")]
        [Description("Demonstrates how to use TestContext to access test metadata.")]
        public async Task TestWithContextInfo()
        {
            var context = TestContext.Current;
            
            Console.WriteLine($"Running test: {context.DisplayName}");
            Console.WriteLine($"Description: {context.Properties.GetValueOrDefault("Description", "None")}");
            Console.WriteLine($"Priority: {context.Properties.GetValueOrDefault("Priority", "Unknown")}");
            
            // Show that items can be used for state sharing
            context.Items["StartTime"] = DateTime.Now;
            
            await Task.Delay(100);
            
            if (context.Items.TryGetValue("StartTime", out var start))
            {
                Console.WriteLine($"Test started at: {start}");
            }
        }

        [Fact]
        [Timeout(1000)]
        public async Task TestCancellationViaContext()
        {
            var ct = TestContext.Current.CancellationToken;
            
            Console.WriteLine("Starting long operation...");
            try
            {
                // This will be cancelled when the 1s timeout is reached
                await Task.Delay(5000, ct);
                Console.WriteLine("Operation completed (unexpectedly!)");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was correctly cancelled via TestContext.CancellationToken");
            }
        }
    }
}
