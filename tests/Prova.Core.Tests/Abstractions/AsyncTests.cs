using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Core.Tests
{
    public class AsyncTests : IAsyncLifetime
    {
        private bool _initialized;

        public async Task InitializeAsync()
        {
            await Task.Delay(10); // Simulate work
            _initialized = true;
            Console.WriteLine("AsyncTests Initialized");
        }

        public async Task DisposeAsync()
        {
             await Task.Delay(10);
            Console.WriteLine("AsyncTests Disposed");
        }

        [Fact]
        public void IsInitialized()
        {
            Assert.True(_initialized, "Should be initialized");
        }
    }
}
