using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Prova;

namespace Prova.AspNetCore
{
    /// <summary>
    /// Factory for bootstrapping an application in memory for functional end-to-end tests with Prova.
    /// </summary>
    /// <typeparam name="TEntryPoint">A type in the entry point assembly of the application.</typeparam>
    public class ProvaWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        /// <summary>
        /// Creates a <see cref="HttpClient"/> that can be used to send requests to the application.
        /// </summary>
        public HttpClient CreateProvaClient()
        {
            return CreateClient();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Allow users to override this to perform additional configuration
            base.ConfigureWebHost(builder);
        }
    }
}
