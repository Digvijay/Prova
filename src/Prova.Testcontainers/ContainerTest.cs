using DotNet.Testcontainers.Containers;
using Prova;
using System.Threading.Tasks;

namespace Prova.Testcontainers
{
    /// <summary>
    /// Base class for tests that require a fresh container instance for each test method.
    /// </summary>
    public abstract class ContainerTest
    {
        /// <summary>
        /// The container instance. Available after [Before] hooks run.
        /// </summary>
        public IContainer? Container { get; private set; }

        /// <summary>
        /// Configures the container to be used.
        /// </summary>
        protected abstract IContainer ConfigureContainer();

        [Before]
        public async Task StartContainerAsync()
        {
            Container = ConfigureContainer();
            if (Container != null)
            {
                await Container.StartAsync();
            }
        }

        [After]
        public async Task StopContainerAsync()
        {
            if (Container != null)
            {
                await Container.StopAsync();
                await Container.DisposeAsync();
            }
        }
    }
}
