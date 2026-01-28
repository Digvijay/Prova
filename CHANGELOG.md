# Changelog

## [v0.3.0] - The Governance Update (Draft) 🛡️

**Status:** Alpha / Draft

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
