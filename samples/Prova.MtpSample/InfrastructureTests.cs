using Prova;

namespace Prova.Sample.Tests
{
    /// <summary>
    /// Database fixture for integration tests.
    /// </summary>
    public class DatabaseFixture : IAsyncLifetime, IDisposable
    {
        /// <summary>Gets the connection string.</summary>
        public string ConnectionString { get; private set; } = "";

        /// <summary>Initializes the fixture.</summary>
        public async Task InitializeAsync()
        {
            await Task.Delay(50);
            ConnectionString = "Server=localhost;Database=MtpSample;";
            Console.WriteLine("    [Fixture] Database Initialized 🔌");
        }

        /// <summary>Disposes the fixture resources asynchronously.</summary>
        public async Task DisposeAsync()
        {
            await Task.Delay(50);
            Console.WriteLine("    [Fixture] Database Disposed 🔌");
        }

        /// <summary>Disposes the fixture resources.</summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Infrastructure tests using the database fixture.
    /// </summary>
    public class InfrastructureTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        /// <summary>Initializes a new instance of the <see cref="InfrastructureTests"/> class.</summary>
        /// <param name="fixture">The database fixture.</param>
        public InfrastructureTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>Verifies fixture initialization.</summary>
        [Fact]
        [Description("Verifies that the database fixture is correctly initialized")]
        public void FixtureTest()
        {
            Assert.Contains("localhost", _fixture.ConnectionString);
        }

        /// <summary>Long running integration test.</summary>
        [Fact]
        [Trait("Category", "Integration")]
        [Description("A long-running integration test")]
        public static async Task LongIntegrationTest()
        {
            await Task.Delay(100);
            Assert.True(true);
        }
    }
}
