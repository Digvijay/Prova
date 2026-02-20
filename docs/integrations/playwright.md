# Playwright Integration

Prova provides first-class support for UI testing with [Microsoft.Playwright](https://playwright.dev/dotnet/). The `Prova.Playwright` package eliminates boilerplate code by managing the browser lifecycle automatically.

## Getting Started

1. **Install the Package**
   ```bash
   dotnet add package Prova.Playwright
   ```

2. **Install Browsers**
   Ensure Playwright browsers are installed:
   ```bash
   pwsh bin/Debug/net10.0/playwright.ps1 install
   ```
   *Note: This script is copied to your output directory during build.*

3. **Inherit from `PageTest`**
   Create a test class that inherits from `Prova.Playwright.PageTest`. This gives you access to a fresh `Page`, `Context`, and `Browser` for every test.

   ```csharp
   using Prova;
   using Prova.Playwright;
   using System.Threading.Tasks;

   public class MyUiTests : PageTest
   {
       [Fact]
       public async Task VerifyHomePage()
       {
           await Page.GotoAsync("https://example.com");
           await Expect(Page).ToHaveTitleAsync("Example Domain");
       }
   }
   ```

## Lifecycle Management

Prova manages the Playwright resources efficiently:

- **Playwright Instance**: Created once per assembly/process (lazy).
- **Browser**: Launched once (default: Headless Chromium). Configurable via `TestContext` or global config (coming soon).
- **BrowserContext**: A new isolated context is created **before every test**.
- **Page**: A new page is created **before every test**.

After each test, the Page and Context are closed, ensuring clean state for the next test.

## Configuration

(Coming Soon)
- Browser selection (Chromium, Firefox, WebKit)
- Headless mode toggle
- Viewport size
- Record video/trace
