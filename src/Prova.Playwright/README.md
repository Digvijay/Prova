# Prova.Playwright

Browser testing for [Prova](https://github.com/Digvijay/Prova), built on
[Microsoft.Playwright](https://playwright.dev/dotnet/).

## Install

```xml
<PackageReference Include="Prova.Playwright" Version="0.6.0" />
```

Install the browsers once before the first run:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

## Usage

Derive from `PageTest`. It manages the Playwright, browser, context, and page lifetimes for you:
the browser is launched once per run (`[BeforeAll]`/`[AfterAll]`) and a fresh context and page are
created for every test (`[Before]`/`[After]`), so tests stay isolated without paying the launch
cost each time.

```csharp
using Prova;
using Prova.Playwright;

public class HomePageTests : PageTest
{
    [Fact]
    public async Task Title_Is_Correct()
    {
        await Page.GotoAsync("https://example.com");

        Assert.Equal("Example Domain", await Page.TitleAsync());
    }
}
```

## Exposed members

| Member | Description |
| --- | --- |
| `Page` | A fresh `IPage` per test |
| `Context` | A fresh `IBrowserContext` per test |
| `Browser` | The shared `IBrowser` for the run |
| `Playwright` | The shared `IPlaywright` driver |

The browser defaults to headless Chromium.

## Documentation

Full guide: <https://prova.digvijay.dev/integrations/playwright>

## License

MIT
