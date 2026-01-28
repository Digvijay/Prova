using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Demo
{
    public class TimeoutSample
    {
        [Fact]
        [Timeout(500)]
        [Description("This test will fail because it takes longer than 500ms.")]
        public async Task Fast_Test_Should_Pass()
        {
            await Task.Delay(100);
        }

        [Fact]
        [Timeout(200)]
        [Description("This test is expected to TIMEOUT.")]
        public async Task Slow_Test_Should_Fail()
        {
            await Task.Delay(5000);
        }
    }
}
