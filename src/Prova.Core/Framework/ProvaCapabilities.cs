using System.Collections.Generic;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Extensions.TrxReport.Abstractions;

namespace Prova
{
    /// <summary>
    /// Defines the capabilities of the Prova test framework.
    /// </summary>
    public sealed class ProvaCapabilities : ITestFrameworkCapabilities
    {
        /// <inheritdoc />
        public IReadOnlyCollection<ITestFrameworkCapability> Capabilities => new ITestFrameworkCapability[] 
        { 
            new ProvaTrxReportCapability(),
            new DiscoveryCapability()
        };
    }

    internal sealed class ProvaTrxReportCapability : ITrxReportCapability 
    {
        public bool IsSupported => true;
        public void Enable() { }
    }

    internal sealed class DiscoveryCapability : ITestFrameworkCapability { }
}
