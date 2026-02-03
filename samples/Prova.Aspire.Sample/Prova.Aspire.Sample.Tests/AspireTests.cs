using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Prova;

namespace Prova.Aspire.Sample.Tests
{
    public class AspireTests : IAsyncLifetime
    {
        private DistributedApplication? _app;

        public async Task InitializeAsync()
        {
            // Create the builder wrapped in a way suitable for tests
            // Note: DistributedApplicationTestingBuilder handles finding the app host entry point
            var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Prova_Aspire_Sample_AppHost>();
            
            // Build the application
            _app = await builder.BuildAsync();
            
            // Start the application
            await _app.StartAsync();
        }

        [Fact]
        public async Task Redis_Resource_Is_Running()
        {
            Assert.NotNull(_app, "DistributedApplication should not be null");

            // Verify we can get the connection string for Redis
            var connectionString = await _app.GetConnectionStringAsync("cache");
            
            Assert.NotNull(connectionString, "Redis connection string should not be null");
            Assert.True(connectionString.Contains("redis"), "Connection string should contain redis");
        }

        public async Task DisposeAsync()
        {
            if (_app != null)
            {
                await _app.DisposeAsync();
            }
        }
    }
}
