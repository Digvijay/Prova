using System.Net.Http.Json;
using Prova;
using Prova.Core;
using Prova.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace Prova.AspNetCore.Sample.Tests
{
    public class WebApplicationTests
    {
        private readonly HttpClient _client;

        [ConfigureServices]
        public static void Configure(ProvaServiceProvider services)
        {
            services.AddSingleton<WebApplicationFactory<Program>>(() => {
                var factory = new ProvaWebApplicationFactory<Program>();
                var contentRoot = global::System.IO.Path.GetFullPath(global::System.IO.Path.Combine(global::System.AppContext.BaseDirectory, "..", "..", ".."));
                return factory.WithWebHostBuilder(builder => {
                    builder.UseContentRoot(contentRoot);
                });
            });
            services.AddTransient<HttpClient>(() => services.Get<WebApplicationFactory<Program>>().CreateClient());
        }

        public WebApplicationTests(HttpClient client)
        {
            _client = client;
        }

        [Fact]
        public async Task Get_Root_Returns_Greeting()
        {
            var response = await _client.GetStringAsync("/");
            Assert.Equal("Hello from Prova ASP.NET Core!", response);
        }

        [Fact]
        public async Task Get_Items_Returns_Success()
        {
            var response = await _client.GetFromJsonAsync("/", SharedJsonContext.Default.StringArray);
            Assert.NotNull(response);
            Assert.Equal(2, response!.Length);
            Assert.Equal("Item1", response[0]);
        }
    }

    [System.Text.Json.Serialization.JsonSourceGenerationOptions(WriteIndented = true)]
    [System.Text.Json.Serialization.JsonSerializable(typeof(string[]))]
    internal sealed partial class SharedJsonContext : System.Text.Json.Serialization.JsonSerializerContext
    {
    }
}
