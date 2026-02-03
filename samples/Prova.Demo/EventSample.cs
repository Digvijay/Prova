using System;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class EventSample : ITestStartEventReceiver, ITestEndEventReceiver
    {
        public Task OnTestStartAsync(ProvaTest test)
        {
            Console.WriteLine($"[EVENT] Test starting: {test.DisplayName}");
            return Task.CompletedTask;
        }

        public Task OnTestEndAsync(ProvaTest test, TestResult result, long durationMs)
        {
            Console.WriteLine($"[EVENT] Test ended: {test.DisplayName}, Result: {result}, Duration: {durationMs}ms");
            return Task.CompletedTask;
        }
    }

    public class EventUsageTests
    {
        [Fact]
        public void EventTest1()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task EventTest2()
        {
            await Task.Delay(50);
            Assert.True(true);
        }
    }
}
