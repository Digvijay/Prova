# Prova.AspNetCore

In-memory ASP.NET Core integration testing for [Prova](https://github.com/Digvijay/Prova), built on
`Microsoft.AspNetCore.Mvc.Testing`.

## Install

```xml
<PackageReference Include="Prova.AspNetCore" Version="0.6.0" />
```

## Usage

`ProvaWebApplicationFactory<TEntryPoint>` boots your application in memory. `TEntryPoint` is any
type from the application's entry point assembly — usually `Program`.

```csharp
using Prova;
using Prova.AspNetCore;

public class HealthEndpointTests
{
    [Fact]
    public async Task Health_Returns_Ok()
    {
        using var factory = new ProvaWebApplicationFactory<Program>();
        using var client = factory.CreateProvaClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

## Customising the host

Derive from the factory and override `ConfigureWebHost` to replace services or settings:

```csharp
public sealed class TestAppFactory : ProvaWebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services => services.AddSingleton<IClock, FixedClock>());
    }
}
```

## Documentation

Full guide: <https://prova.digvijay.dev/integrations/aspnet-core>

## License

MIT
