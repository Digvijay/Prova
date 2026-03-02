using System;
using System.Threading;
using System.Threading.Tasks;
using Prova;

namespace Prova.Demo
{
    public class StaTests
    {
        [Fact]
        [Executor(typeof(StaExecutor))]
        public void Test_Running_In_STA()
        {
            var apartment = Thread.CurrentThread.GetApartmentState();
            Console.WriteLine($"[STA] Test running in: {apartment} (Thread ID: {Environment.CurrentManagedThreadId})");
            
            if (global::System.OperatingSystem.IsWindows())
            {
                Assert.Equal(ApartmentState.STA, apartment);
            }
        }

        [Fact]
        public void Test_Running_In_MTA_By_Default()
        {
            var apartment = Thread.CurrentThread.GetApartmentState();
            Console.WriteLine($"[MTA] Test running in: {apartment} (Thread ID: {Environment.CurrentManagedThreadId})");
        }
    }
}
