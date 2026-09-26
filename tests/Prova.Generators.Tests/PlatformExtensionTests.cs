// Test definitions use Prova's own [Fact]/[Theory], dogfooding the framework under test.

namespace Prova.Generators.Tests
{
    /// <summary>
    /// Prova emits its own entry point, and used to register only crash dump and hang dump by hand.
    /// Every other Microsoft.Testing.Platform extension a project referenced (code coverage, TRX,
    /// retry) was silently dropped, and its command-line options were then rejected as unknown,
    /// which failed the run with exit code 5.
    /// </summary>
    public class PlatformExtensionTests
    {
        private const string TestClass = @"
using Prova;
public class MyTests
{
    [Fact]
    public void Works() { }
}";

        // What Microsoft.Testing.Platform.MSBuild generates into every test project that references it.
        private const string SelfRegistered = @"
namespace TestProject
{
    internal static class SelfRegisteredExtensions
    {
        public static void AddSelfRegisteredExtensions(this object builder, string[] args) { }
    }
}";

        private const string CoverageHook = @"
namespace Microsoft.Testing.Extensions.CodeCoverage
{
    public static class TestingPlatformBuilderHook { }
}";

        [Fact]
        public void Calls_the_platform_self_registration_hook_when_present()
        {
            var generated = GeneratorVerifier.Generate(TestClass + SelfRegistered);

            Assert.Contains("global::TestProject.SelfRegisteredExtensions.AddSelfRegisteredExtensions(builder, args);", generated);
        }

        [Fact]
        public void Does_not_register_dump_providers_twice_when_the_hook_already_does()
        {
            var generated = GeneratorVerifier.Generate(TestClass + SelfRegistered);

            Assert.DoesNotContain("builder.AddCrashDumpProvider();", generated);
            Assert.DoesNotContain("builder.AddHangDumpProvider();", generated);
        }

        [Fact]
        public void Falls_back_to_manual_registration_without_the_hook()
        {
            var generated = GeneratorVerifier.Generate(TestClass);

            Assert.Contains("builder.AddCrashDumpProvider();", generated);
            Assert.Contains("builder.AddHangDumpProvider();", generated);
            Assert.DoesNotContain("AddSelfRegisteredExtensions", generated);
        }

        [Fact]
        public void Leaves_coverage_switch_for_the_platform_extension_when_it_is_referenced()
        {
            var generated = GeneratorVerifier.Generate(TestClass + SelfRegistered + CoverageHook);

            Assert.Contains("var filteredArgs = args;", generated);
            Assert.DoesNotContain("args.Where(a => a != \"--coverage\")", generated);
        }

        [Fact]
        public void Keeps_prova_lcov_coverage_when_no_platform_coverage_extension_is_referenced()
        {
            var generated = GeneratorVerifier.Generate(TestClass + SelfRegistered);

            Assert.Contains("args.Where(a => a != \"--coverage\")", generated);
        }

        [Fact]
        public void Coverage_extension_without_the_hook_is_not_treated_as_registered()
        {
            // The extension only receives the switch if the hook that registers it is called.
            var generated = GeneratorVerifier.Generate(TestClass + CoverageHook);

            Assert.Contains("args.Where(a => a != \"--coverage\")", generated);
        }

        [Fact]
        public void ProvaDisablePlatformCoverage_strips_the_switch_but_keeps_other_extensions_registered()
        {
            var generated = GeneratorVerifier.Generate(
                TestClass + SelfRegistered + CoverageHook,
                new Dictionary<string, string>
                {
                    ["build_property.ProvaDisablePlatformCoverage"] = "true"
                });

            Assert.Contains("global::TestProject.SelfRegisteredExtensions.AddSelfRegisteredExtensions(builder, args);", generated);
            Assert.Contains("args.Where(a => a != \"--coverage\")", generated);
        }
    }
}
