# Testcontainers Integration

Prova supports integration testing with [Testcontainers for .NET](https://dotnet.testcontainers.org/) through the `Prova.Testcontainers` library.

## Getting Started

1. **Install the Packages**
   ```bash
   dotnet add package Prova.Testcontainers
   dotnet add package Testcontainers.PostgreSql # or other modules
   ```

2. **Per-Test Container**
   Inherit from `ContainerTest` to get a fresh container for every test.

   ```csharp
   using Prova;
   using Prova.Testcontainers;
   using Testcontainers.PostgreSql;
   using DotNet.Testcontainers.Containers;

   public class MyDbTest : ContainerTest
   {
       protected override IContainer ConfigureContainer()
       {
           return new PostgreSqlBuilder()
               .WithDatabase("testdb")
               .Build();
       }

       [Fact]
       public async Task CanConnect()
       {
           // Container is started automatically before this test
           var connString = (Container as PostgreSqlContainer).GetConnectionString();
           // ... use connection
       }
   }
   ```

3. **Shared Container (Fixture)**
   Use `ContainerFixture` to share a container across a test class.

   ```csharp
   public class PostgresFixture : ContainerFixture
   {
       protected override IContainer ConfigureContainer()
       {
           return new PostgreSqlBuilder().Build();
       }
       
       public string ConnectionString => (Container as PostgreSqlContainer)?.GetConnectionString();
   }

   public class SharedTest : IClassFixture<PostgresFixture>
   {
       private readonly PostgresFixture _fixture;
       public SharedTest(PostgresFixture fixture) => _fixture = fixture;

       [Fact]
       public void Test1() 
       {
           // _fixture.Container is running
       }
   }
   ```

## Lifecycle Management
- **ContainerTest**: `StartAsync` is called before each test. `DisposeAsync` is called after each test.
- **ContainerFixture**: `StartAsync` is called once before the class. `DisposeAsync` is called once after all tests in the class.

## Requirements
- Docker execution environment (e.g., Docker Desktop, OrbStack, or a Docker-enabled CI runner).
