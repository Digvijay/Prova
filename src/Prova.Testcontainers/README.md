# Prova.Testcontainers

Docker-backed integration testing for [Prova](https://github.com/Digvijay/Prova), built on
[Testcontainers for .NET](https://dotnet.testcontainers.org/).

## Install

```xml
<PackageReference Include="Prova.Testcontainers" Version="0.6.0" />
```

Requires a running Docker daemon.

## Choosing a base class

| Base class | Container lifetime | Use when |
| --- | --- | --- |
| `ContainerTest` | One fresh container per test method | Tests mutate state and must not see each other's writes |
| `ContainerFixture` | One container shared across the class | Startup is expensive and tests are read-only or self-isolating |

Both call your `ConfigureContainer()` override to build the container.

## Per-test container

```csharp
using DotNet.Testcontainers.Builders;
using Prova;
using Prova.Testcontainers;

public class RedisTests : ContainerTest
{
    protected override IContainer ConfigureContainer() =>
        new ContainerBuilder()
            .WithImage("redis:7-alpine")
            .WithPortBinding(6379, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();

    [Fact]
    public async Task Container_Is_Running()
    {
        Assert.NotNull(Container);
        Assert.Equal(TestcontainersStates.Running, Container!.State);
    }
}
```

## Shared container

```csharp
public sealed class PostgresFixture : ContainerFixture
{
    protected override IContainer ConfigureContainer() =>
        new ContainerBuilder()
            .WithImage("postgres:16-alpine")
            .WithEnvironment("POSTGRES_PASSWORD", "test")
            .Build();
}
```

`ContainerFixture` implements `IAsyncLifetime`, so the container starts once before the first test
in the class and is stopped and disposed afterwards.

## Documentation

Full guide: <https://prova.digvijay.dev/integrations/testcontainers>

## License

MIT
