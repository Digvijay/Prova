using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Prova;

namespace Prova.Playwright
{
    /// <summary>
    /// Base class for Playwright tests.
    /// Manages the lifecycle of Playwright, Browser, Context, and Page.
    /// </summary>
    public abstract class PageTest
    {
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;

        /// <summary>
        /// Gets the current Playwright instance.
        /// </summary>
        public IPlaywright Playwright => _playwright ?? throw new InvalidOperationException("Playwright not initialized.");

        /// <summary>
        /// Gets the current Browser instance.
        /// </summary>
        public IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not initialized.");

        /// <summary>
        /// Gets the current Browser Context.
        /// </summary>
        public IBrowserContext Context { get; private set; } = null!;

        /// <summary>
        /// Gets the current Page.
        /// </summary>
        public IPage Page { get; private set; } = null!;

        [BeforeAll]
        public static async Task GlobalSetup()
        {
            if (_playwright == null)
            {
                _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            }

            if (_browser == null)
            {
                // TODO: Load configuration from TestContext or Config
                // Defaulting to Headless Chromium for now
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true
                });
            }
        }

        [AfterAll]
        public static async Task GlobalTeardown()
        {
            if (_browser != null)
            {
                await _browser.DisposeAsync();
                _browser = null;
            }

            if (_playwright != null)
            {
                _playwright.Dispose();
                _playwright = null;
            }
        }

        [Before]
        public async Task Setup()
        {
            if (_browser == null)
            {
                 // Should have been initialized by BeforeAll, but double check
                 // In case of parallel execution or missed hook (unlikely)
                 await GlobalSetup();
            }

            Context = await _browser!.NewContextAsync();
            Page = await Context.NewPageAsync();
        }

        [After]
        public async Task Teardown()
        {
            if (Page != null)
            {
                await Page.CloseAsync();
            }

            if (Context != null)
            {
                await Context.CloseAsync();
            }
        }
    }
}
