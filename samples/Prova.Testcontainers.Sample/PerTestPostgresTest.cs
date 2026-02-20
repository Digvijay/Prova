using Prova;
using Prova.Testcontainers;
using Testcontainers.PostgreSql;
using System.Threading.Tasks;
using Npgsql;
using DotNet.Testcontainers.Containers;

namespace Prova.Testcontainers.Sample
{
    public class PerTestPostgresTest : ContainerTest
    {
        protected override IContainer ConfigureContainer()
        {
            // Usually you might want lighter containers for per-test, but for demo:
            return new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("isolated_db")
                .WithUsername("postgres")
                .WithPassword("password")
                .Build();
        }

        [Fact]
        public async Task Test1_IsIsolated()
        {
             var pgContainer = Container as PostgreSqlContainer;
             var connString = pgContainer?.GetConnectionString();
             
             Assert.NotNull(connString);
             
             using var conn = new NpgsqlConnection(connString);
             await conn.OpenAsync();
             
             // Create a table
             using var cmd = new NpgsqlCommand("CREATE TABLE foo (id int)", conn);
             await cmd.ExecuteNonQueryAsync();
        }

        [Fact]
        public async Task Test2_IsIsolted_TableShouldNotExist()
        {
             var pgContainer = Container as PostgreSqlContainer;
             var connString = pgContainer?.GetConnectionString();
             
             using var conn = new NpgsqlConnection(connString);
             await conn.OpenAsync();
             
             // Table foo should NOT exist because this is a new container
             // We can check this by querying pg_tables or trying to select
             bool tableExists = false;
             try 
             {
                 using var cmd = new NpgsqlCommand("SELECT * FROM foo", conn);
                 await cmd.ExecuteNonQueryAsync();
                 tableExists = true;
             }
             catch (PostgresException)
             {
                 // Expected: relation "foo" does not exist
                 tableExists = false;
             }
             
             Assert.False(tableExists, "Table 'foo' should not exist in a fresh container");
        }
    }
}
