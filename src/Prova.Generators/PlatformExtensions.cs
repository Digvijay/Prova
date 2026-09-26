using System.Linq;
using Microsoft.CodeAnalysis;

namespace Prova.Generators
{
    /// <summary>
    /// What Microsoft.Testing.Platform extensions the test project has opted into.
    /// </summary>
    /// <remarks>
    /// Prova sets <c>GenerateTestingPlatformEntryPoint=false</c> so that it can emit its own entry
    /// point, and that entry point used to register only the crash-dump and hang-dump providers by
    /// hand. Every other extension a user referenced (code coverage, TRX reports, retry) was
    /// silently ignored, and its options were then rejected as unknown.
    ///
    /// Microsoft.Testing.Platform.MSBuild still generates a <c>SelfRegisteredExtensions</c> class
    /// listing every referenced extension's hook, so calling it is how the platform intends a custom
    /// entry point to pick them up.
    /// </remarks>
    internal readonly record struct PlatformExtensions(string? SelfRegisteredExtensionsType, bool HasPlatformCoverage)
    {
        private const string CoverageHook = "Microsoft.Testing.Extensions.CodeCoverage.TestingPlatformBuilderHook";

        public static PlatformExtensions From(Compilation compilation)
        {
            var selfRegistered = compilation
                .GetSymbolsWithName("SelfRegisteredExtensions", SymbolFilter.Type)
                .OfType<INamedTypeSymbol>()
                .FirstOrDefault(t => t.IsStatic && t.GetMembers("AddSelfRegisteredExtensions").OfType<IMethodSymbol>().Any());

            var typeName = selfRegistered?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // Coverage only reaches the platform extension if the hook that registers it is called.
            bool hasCoverage = typeName != null && compilation.GetTypeByMetadataName(CoverageHook) != null;

            return new PlatformExtensions(typeName, hasCoverage);
        }
    }
}
