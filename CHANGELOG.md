# Changelog

## [v0.5.0] - MTP-Native Theory Unrolling

**Released:** 2026-03-02

This release upgrades Prova to a .NET 10 "MTP-Native" testing framework with zero-reflection execution and a streamlined onboarding experience.

### Breaking Changes
-   Prova now auto-generates `Program.g.cs`. If you have a custom `Program.cs`, set `<GenerateProgramFile>false</GenerateProgramFile>` in your `.csproj` to avoid conflicts, or remove your custom entry point.
-   The `.csproj` for test projects now requires `<OutputType>Exe</OutputType>` (set automatically by `Prova.props`).

### Bug Fixes
-   **Theory Unrolling**: Fixed the v0.4.0 bug where `[Theory]` + `[InlineData]` could fail under Native AOT. Each `[InlineData]` row is now "unrolled" into a unique, statically-generated, parameterless test method at compile-time. Display names now show actual data values (e.g., `Add(1, 2, 3)`) instead of the opaque `Row_N` format.
-   **String Quote Escaping**: Fixed a compile error in generated code when string items in `[InlineData]` contained quotes (`"`). They are now properly escaped into valid C# literals (`\"`).

### New Features
-   **MTP-Native Execution**: Replaced the legacy VSTest bridge with a native Microsoft Testing Platform (MTP) execution model. All MTP CLI arguments (`--list-tests`, `--report-trx`, `--coverage`, `--filter`, `--crashdump`, `--hangdump`) are natively supported.
-   **Static TestRegistry**: The generator now emits a `TestRegistry` class with `Count` and `Discover()` methods, providing a 0ms discovery phase by serving a pre-compiled list of tests to the MTP host.
-   **Auto-Generated Entry Point**: If no `Main` method is detected, the generator emits a `Program.g.cs` containing `await Prova.TestRunnerExecutor.RunAllAsync(args)`, making the test project a self-executing console app by default.
-   **`Prova.Assertions` Namespace**: The `Prova.Assertions` namespace is now correctly exported, so `using Prova.Assertions;` works immediately as per documentation.
-   **UTF-8 Encoding**: The generated entry point forces `Console.OutputEncoding = Encoding.UTF8` to prevent emoji mangling in `[DisplayName]` across PowerShell, bash, and zsh.

### Documentation
-   **README Overhaul**: Rewritten Getting Started guide reflecting `<OutputType>Exe</OutputType>` and `<TargetFramework>net10.0</TargetFramework>` requirements.
-   **MTP CLI Docs**: Documented MTP-powered CLI arguments (`--list-tests`, `--report-trx`, `--coverage`, `--filter`, `--crashdump`, `--hangdump`).

---


## [v0.4.0] - The Integrations Update 🔌

**Released:** 2026-02-20

This release adds first-class integration packages, global lifecycle hooks, and a VitePress documentation site.

### ✨ New Features
-   **FsCheck Integration**: New `Prova.FsCheck` package with `[Property]` attribute for property-based testing.
-   **Global Lifecycle Hooks**: Added `[BeforeEvery]` and `[AfterEvery]` attributes for cross-cutting concerns.
-   **Documentation Site**: Full VitePress documentation deployed to [prova.digvijay.dev](https://prova.digvijay.dev).
-   **Migration Analyzers**: Added code fixers for MSTest (`[TestMethod]`, `[DataRow]`, `[TestInitialize]`, etc.).

### 🛠️ Improvements
-   **.NET 10 Compatibility**: Fixed `dotnet test` MTP compatibility on .NET 10 SDK.
-   **Zero-Warning Build**: Resolved all RS1032/RS1033 analyzer warnings for clean CI.
-   **NuGet Packaging**: All integration packages (`Prova.AspNetCore`, `Prova.Playwright`, `Prova.Testcontainers`, `Prova.FsCheck`) are now independently packable with proper metadata.

### 📦 New Packages
| Package | Description |
|---------|-------------|
| `Prova` | Core framework |
| `Prova.AspNetCore` | ASP.NET Core integration |
| `Prova.Playwright` | Playwright browser testing |
| `Prova.Testcontainers` | Docker container testing |
| `Prova.FsCheck` | Property-based testing |

---

## [v0.3.0] - The Governance Update 🛡️

**Released:** 2026-01-28

This update introduces "Governance Features" for mission-critical and high-performance applications.

### ✨ New Features
-   **Memory Governance**: Added `[MaxAlloc(bytes)]` attribute.
    -   Enables strict, zero-overhead enforcement of heap allocation budgets per test.
    -   Powered by `GC.GetAllocatedBytesForCurrentThread()` wrapped in `try/finally` blocks.
-   **Assertions Parity**: Completed the implementation of `Assert.Throws`, `Assert.Contains`, `Assert.Empty`, `Assert.IsType`, and `Assert.Same`.

### 🛠️ Improvements
-   **Documentation**: Initial support for verifying XML documentation completeness in CI.

## [v0.2.0] - The Enterprise Update 🚀

**Released:** 2026-01-28

This release transforms Prova from a "research project" into a standard-complaint, enterprise-ready testing framework for the .NET 10 era.

### ✨ New Features
-   **Explicit Concurrency Control**: Added `[Parallel(max: N)]` attribute to prevent thread pool starvation.
    -   Implemented "Bounded Scheduler" using `SemaphoreSlim` in both Console and MTP modes.
    -   Default concurrency is now `Environment.ProcessorCount` (safe) instead of Unbounded (dangerous).
-   **Output Capture**: `ITestOutputHelper` is now fully fully plumbed.
    -   Logs written during tests are captured and displayed in the runner output.
    -   MTP Adapter attaches logs to `StandardOutput`, making them visible in Visual Studio / VS Code.
-   **Dynamic Data**: Added `[ClassData(typeof(T))]` for complex, reusable data sources.
-   **AOT Dependency Injection**: Added `[TestDependency]` for elegant, reflection-free, compile-time service injection.
-   **Issue Templates**: Added structured `bug_report.md` and `feature_request.md`.

### 🛠️ Improvements
-   **MTP Integration**: Upgraded to `Microsoft.Testing.Platform` v2.0.2 stable.
-   **Code Coverage**: Integrated `Microsoft.Testing.Extensions.CodeCoverage` (v18.3.2) with Cobertura support.
-   **Attributes**: `ProvaTest` and `TestMethodModel` updated to support concurrency metadata.

---

## [v0.1.1] - Automation Patches 🤖

**Released:** 2026-01-27

-   **CI/CD**: Fixed GitHub Actions workflow to correctly extract version numbers from Git tags (`v*`).
-   **Publishing**: Automated NuGet publication on tag push.
-   **Versioning**: Fixed `dotnet pack` variable scoping issues.

---

## [v0.1.0] - Initial Release (MVP) 🌱

**Released:** 2026-01-26

-   **Core**: Initial release of the Prova framework.
-   **AOT**: Zero-reflection, Source Generator-based discovery (`[Fact]`, `[Theory]`).
-   **Parity**: Support for `[InlineData]`, `[MemberData]`, `IClassFixture<T>`, `IAsyncLifetime`.
-   **Runners**:
    -   `dotnet run -- --simple`: Lightweight Console Runner.
    -   `dotnet test`: Basic MTP integration.
