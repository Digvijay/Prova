using DotNet.Testcontainers.Containers;
using Prova;
using System;
using System.Threading.Tasks;

namespace Prova.Testcontainers
{
    /// <summary>
    /// Base class for shared container fixtures (IClassFixture).
    /// </summary>
    public abstract class ContainerFixture : IAsyncLifetime, IDisposable
    {
        public IContainer? Container { get; private set; }

        protected abstract IContainer ConfigureContainer();

        public async Task InitializeAsync()
        {
            if (Container != null) return;
            Container = ConfigureContainer();
            if (Container != null)
            {
                await Container.StartAsync();
            }
        }

        public async Task DisposeAsync()
        {
            if (Container != null)
            {
                await Container.StopAsync();
                await Container.DisposeAsync();
            }
        }

        public void Dispose()
        {
            // No-op, managed by DisposeAsync
            GC.SuppressFinalize(this);
        }
    }
}
