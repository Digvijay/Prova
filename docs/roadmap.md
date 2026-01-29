# Roadmap: v0.4.0 (Developer Experience)

We are committed to bringing "Dev-First" features to Prova while maintaining our "Ops-First" stability.

## 🛠️ Automated Migration (Code Fixers)
**Goal**: Reduce migration friction to near-zero.
- [ ] **Analyzer**: Detect `using Xunit;` and suggest `using Prova;`.
- [ ] **Fixer**: Automatically convert incompatible patterns (e.g., `ICollectionFixture` -> `IClassFixture`).

## 📊 Integrated Code Coverage
**Goal**: Native coverage without external tool complexity.
- [ ] **LCOV Support**: Emit coverage reports during source generation.
- [ ] **Native Collector**: Hook into the static entry point to track execution paths.

## 🪝 Lifecycle Hooks
**Goal**: Granular control over test execution.
- [ ] `[Before(Test)]` / `[After(Test)]`
- [ ] `[Before(Assembly)]` / `[After(Assembly)]`
