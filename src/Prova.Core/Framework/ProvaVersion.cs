using System.Reflection;

namespace Prova
{
    /// <summary>
    /// The single source of truth for the version Prova reports to the test platform.
    /// </summary>
    /// <remarks>
    /// This is read from assembly metadata rather than written as a literal. The adapter
    /// previously hardcoded its version string, which then drifted: it still reported 0.5.0
    /// after the package had moved on. Anything a human has to remember to update twice
    /// eventually disagrees with itself.
    /// </remarks>
    internal static class ProvaVersion
    {
        /// <summary>Gets the informational version of the assembly Prova ships in.</summary>
        public static string Value { get; } = ReadVersion();

        private static string ReadVersion()
        {
            var assembly = typeof(ProvaVersion).Assembly;

            var informational = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            if (!string.IsNullOrWhiteSpace(informational))
            {
                // Strip any source-revision suffix that the SDK appends, e.g. "0.6.0+abc1234".
                var plus = informational!.IndexOf('+');
                return plus > 0 ? informational.Substring(0, plus) : informational;
            }

            return assembly.GetName().Version?.ToString() ?? "0.0.0";
        }
    }
}
