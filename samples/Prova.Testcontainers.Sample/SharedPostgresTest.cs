using Prova;
using Prova.Testcontainers;
using Testcontainers.PostgreSql;
using System.Threading.Tasks;
using Npgsql;
using DotNet.Testcontainers.Containers;

namespace Prova.Testcontainers.Sample
{
    public class PostgresFixture : ContainerFixture
    {
        protected override IContainer ConfigureContainer()
        {
            return new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("password")
                .Build();
        }
        
        public string ConnectionString => (Container as PostgreSqlContainer)?.GetConnectionString() ?? "";
    }

    public class SharedPostgresTest : IClassFixture<PostgresFixture>
    {
        private readonly PostgresFixture _fixture;

        public SharedPostgresTest(PostgresFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CanConnectToSharedContainer()
        {
             var connString = _fixture.ConnectionString;
             Assert.False(string.IsNullOrEmpty(connString), "Connection string should not be empty");

             using var conn = new NpgsqlConnection(connString);
             await conn.OpenAsync();
             Assert.True(conn.State == System.Data.ConnectionState.Open, "Connection should be open");

             using var cmd = new NpgsqlCommand("SELECT 1", conn);
             var result = await cmd.ExecuteScalarAsync();
             Assert.Equal(1, result);
        }
        
        [Fact]
        public async Task CanUseSameContainerInAnotherTest()
        {
             var connString = _fixture.ConnectionString;
             using var conn = new NpgsqlConnection(connString);
             await conn.OpenAsync();
             
             // Check if database persists (if we wrote data, it would be there)
             using var cmd = new NpgsqlCommand("SELECT datname FROM pg_database WHERE datname = 'testdb'", conn);
             var dbName = await cmd.ExecuteScalarAsync();
             Assert.Equal("testdb", dbName);
        }
    }
}
