using System.Threading.Tasks;
using Prova;
using Prova.Playwright;

namespace Prova.Playwright.Sample
{
    public class MyUiTest : PageTest
    {
        [Fact]
        public async Task VerifyExampleDomain()
        {
            await Page.GotoAsync("https://example.com");
            var title = await Page.TitleAsync();
            Assert.Equal("Example Domain", title);
        }

        [Fact]
        public async Task VerifyContent()
        {
            await Page.GotoAsync("https://example.com");
            var content = await Page.ContentAsync();
            Assert.Contains("Example Domain", content);
        }
    }
}
