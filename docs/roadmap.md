# Roadmap: v0.4.0 (Developer Experience)

We are committed to bringing "Dev-First" features to Prova while maintaining our "Ops-First" stability.

## 🛠️ Automated Migration (Code Fixers)
**Goal**: Reduce migration friction to near-zero.
- [x] **xUnit Analyzer**: Detect `using Xunit;` and suggest `using Prova;`.
- [x] **NUnit Analyzer**: Detect `[TestFixture]`, `[Test]` and suggest Prova equivalents. [NEW]
- [x] **MSTest Analyzer**: Detect `[TestClass]`, `[TestMethod]` and suggest Prova equivalents. [NEW]
- [x] **Fixer**: Automatically convert incompatibilities (`ICollectionFixture`, `TestContext` access).

## 📊 Integrated Code Coverage
**Goal**: Native coverage without external tool complexity.
- [x] **LCOV Support**: Emit coverage reports during source generation.
- [x] **Native Collector**: Hook into the static entry point.

## 🪝 Lifecycle & Control
**Goal**: Granular control over test execution and data.
- [x] **Hooks**: `[Before]`, `[After]`, `[BeforeClass]`, `[AfterClass]`, `[BeforeAssembly]`, `[AfterAssembly]`, `[BeforeAll]`, `[BeforeEach]`, `[AfterAll]`, `[AfterEach]`. [NEW]
- [x] **Global Hooks**: `[BeforeEvery(Test)]`, `[BeforeEvery(Class)]` for cross-cutting concerns. [NEW]
- [x] **Parallelism**: `[NotInParallel]` attribute with Constraint Keys (e.g. "Database"). [NEW]
- [x] **Parallel Groups**: `[ParallelGroup("key")]` to run specific groups in parallel while isolating others. [NEW]
- [x] **Parallel Limiter**: `[ParallelLimiter<T>]` to restrict concurrency for specific resources. [NEW]
- [x] **Flow Control**: `[Retry(3)]`, `[Repeat(3)]`, and `[Timeout(3000)]`. [NEW]
- [x] **Culture**: `[Culture("en-US")]` to run tests under specific culture settings. [NEW]
- [x] **Global Policies**: Assembly-level defaults for Retry, Timeout, and Parallelism. [NEW]
- [x] **Data Sources**: `[ClassDataSource]`, `[MethodDataSource]` for flexible DI. [NEW]
- [x] **Generic Tests**: `[Test]` on generic classes/methods with type arguments. [NEW]
- [x] **Properties**: `[Property("key", "val")]` for test filtering and metadata. [NEW]
- [x] **Filters**: CLI `dotnet run --treenode-filter` support. [NEW]
- [x] **Events**: `ITestStartEventReceiver`, `ITestEndEventReceiver` for lifecycle observation. [NEW]
- [x] **Custom Executors**: `ITestExecutor` and `IHookExecutor` (AOT Safe). [NEW]
- [x] **Scripting**: Support for file-based test execution (`dotnet run tests.cs`). [NEW]
- [x] **TestContext**: Refactor to expose `Events`, `Metadata`, and `Result` properties clearly. [NEW]
- [x] **State Sharing**: `StateBag` to pass data from Discovery -> Execution phase. [NEW]

## 🔌 Extensibility & Advanced Scenarios
**Goal**: Allow users to customize every aspect of the engine.
- [x] **Test Variants**: Dynamic test generation (e.g. valid vs invalid inputs) via `[TestVariant]`. [NEW]
- [x] **Dynamic Tests**: Imperative test generation via `DynamicTestBuilder` (code-based). [NEW]
- [x] **Detailed Configuration**: `testconfig.json` support for environment-specific settings. [NEW]
- [x] **Combinatorial Tests**: `[Matrix]` attribute for testing all combinations of input data. [NEW]
- [x] **Data Generators**: `[DataSourceGenerator]` base class for custom data sources. [NEW]
- [x] **Formatters**: `[ArgumentDisplayFormatter]` to control argument presentation in logs/UI. [NEW]
- [x] **Display Names**: `[DisplayName("...")]` with placeholders for arguments (`{0}`). [NEW]
- [x] **Logging**: `ITestLogger` infrastructure and `TestContext.Logger` sinks. [NEW]
- [x] **Debugging Support**: Crash Dumps and Hang Dumps integration (via MTP extensions). [NEW]

## 💉 Dependency Injection
**Goal**: First-class DI support.
- [x] **Constructor Injection**: Support injecting services into Test Classes. [NEW]
- [x] **Class Factories**: `IClassConstructor` helper for complex instantiation logic. [NEW]
- [x] **DI Data Sources**: `[DependencyInjectionDataSource]` to source data from containers. [NEW]
- [x] **ASP.NET Core**: `WebApplicationFactory` integration for integration testing. [NEW]

## 📸 Artifacts & Observability
**Goal**: First-class support for test outputs.
- [x] **Attachments**: `TestContext.Output.AttachArtifact(...)` for files/screenshots. [NEW]
- [x] **Session Artifacts**: Logs/Reports that span the entire test run. [NEW]
- [x] **CI/CD Integration**: Native support for GitHub Actions and Azure DevOps reporting. [NEW]

## 🧩 Integrations & Ecosystem
**Goal**: Seamless integration with the .NET ecosystem.
- [x] **Playwright**: First-class support for UI testing with `PageTest` base class. [NEW]
- [x] **FsCheck**: Property-based testing integration (`[Property]`). [NEW]
- [x] **Testcontainers**: Orchestration for complex dependencies (Docker, Postgres). [NEW]
- [ ] **Aspire**: Integration with .NET Aspire for microservices testing. [NEW]

## 📚 Documentation & Guides
**Goal**: comprehensive learning resources.
- [ ] **Best Practices**: Guide on Naming, Organization, and Assertions. [NEW]
- [ ] **Performance**: Guide on `[ACT]` mode, lightweight data, and parallel optimization. [NEW]
- [ ] **Orchestration**: Guide on managing complex test infrastructure. [NEW]
