using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Prova;

namespace Prova.Aspire.Sample.Tests
{
    /// <summary>
    /// Demonstrates running Prova against a .NET Aspire distributed application.
    /// </summary>
    /// <remarks>
    /// This sample starts a real Redis container, so it requires a container runtime and is
    /// excluded from the solution-wide test run. See the project file for how to run it.
    /// </remarks>
    public class AspireTests : IAsyncLifetime
    {
        private DistributedApplication? _app;

        private DistributedApplication App =>
            _app ?? throw new InvalidOperationException(
                $"{nameof(InitializeAsync)} did not run, so the distributed application was never started.");

        public async Task InitializeAsync()
        {
            var builder = await DistributedApplicationTestingBuilder
                .CreateAsync<Projects.Prova_Aspire_Sample_AppHost>();

            _app = await builder.BuildAsync();
            await _app.StartAsync();
        }

        [Fact]
        public async Task Redis_Resource_Is_Running()
        {
            var connectionString = await App.GetConnectionStringAsync("cache");

            Assert.NotNull(connectionString, "Redis connection string should not be null");
            Assert.True(
                connectionString!.Contains("redis", StringComparison.OrdinalIgnoreCase),
                "Connection string should identify a Redis resource");
        }

        public async Task DisposeAsync()
        {
            if (_app is not null)
            {
                await _app.DisposeAsync();
            }
        }
    }
}
