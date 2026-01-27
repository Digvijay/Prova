using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Verifies async lifetime behavior.
    /// </summary>
    public class AsyncTests : IAsyncLifetime
    {
        private bool _initialized;

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            await Task.Delay(10); // Simulate work
            _initialized = true;
            Console.WriteLine("AsyncTests Initialized");
        }

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
             await Task.Delay(10);
            Console.WriteLine("AsyncTests Disposed");
        }

        /// <summary>
        /// Checks if initialization occurred.
        /// </summary>
        [Fact]
        public void IsInitialized()
        {
            Assert.True(_initialized, "Should be initialized");
        }
    }
}
