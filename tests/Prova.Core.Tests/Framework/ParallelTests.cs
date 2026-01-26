using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Core.Tests.Framework
{
    public class ParallelTest1
    {
        [Fact]
        public static async Task Sleep1()
        {
            await Task.Delay(1000);
        }
    }

    public class ParallelTest2
    {
        [Fact]
        public static async Task Sleep1()
        {
             await Task.Delay(1000);
        }
    }
}
