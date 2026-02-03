using System;
using System.Threading.Tasks;
using Prova.Core;

namespace Prova.Demo
{
    public class ResourceTests
    {
        [Fact]
        [NotInParallel("Database")]
        public async Task Test1()
        {
            Console.WriteLine($"[Test1] Start: {DateTime.Now:HH:mm:ss.fff}");
            await Task.Delay(1000);
            Console.WriteLine($"[Test1] End: {DateTime.Now:HH:mm:ss.fff}");
        }

        [Fact]
        [NotInParallel("Database")]
        public async Task Test2()
        {
            Console.WriteLine($"[Test2] Start: {DateTime.Now:HH:mm:ss.fff}");
            await Task.Delay(1000);
            Console.WriteLine($"[Test2] End: {DateTime.Now:HH:mm:ss.fff}");
        }

        [Fact]
        public async Task Test3_NoConflict()
        {
             // This should run in parallel with the others potentially
            Console.WriteLine($"[Test3] Start: {DateTime.Now:HH:mm:ss.fff}");
            await Task.Delay(500);
            Console.WriteLine($"[Test3] End: {DateTime.Now:HH:mm:ss.fff}");
        }
    }
}
