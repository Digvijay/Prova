using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Core.Tests.Framework
{
    /// <summary>
    /// First class for parallel testing.
    /// </summary>
    public class ParallelTest1
    {
        /// <summary>Async sleep test.</summary>
        [Fact]
        public static async Task Sleep1()
        {
            await Task.Delay(1000);
        }
    }

    /// <summary>
    /// Second class for parallel testing.
    /// </summary>
    public class ParallelTest2
    {
        /// <summary>Async sleep test.</summary>
        [Fact]
        public static async Task Sleep1()
        {
             await Task.Delay(1000);
        }
    }
}
