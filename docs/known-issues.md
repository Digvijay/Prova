# Known issues and resolved defects

This file records every defect found while preparing Prova for open-source review, whether or
not it is fixed. It exists because a project asking to be trusted with other people's test runs
should be explicit about what was wrong with it.

Each fixed entry names the root cause and, where applicable, the test that fails without the fix.

## Severity 1 — the framework reported success while doing nothing

### 1. `dotnet test` did not work at all on the .NET 10 SDK

`global.json` did not opt in to the Microsoft.Testing.Platform runner, so `dotnet test` used the
VSTest path against MTP-only test projects and aborted.

**Fixed.** `global.json` now sets `"test": { "runner": "Microsoft.Testing.Platform" }`.

### 2. `IsTestingPlatformApplication` was inverted, so CI ran zero tests

`dotnet test` uses `IsTestingPlatformApplication` to decide what is runnable. It was `false` on the
test projects and unset on the shipping libraries, which reference MTP. The result was
*"No test projects were found"* on some invocations and *"OutputType is 'Library'"* on others.
CI was green because nothing ran.

**Fixed.** The property is now `false` on the five shipping libraries and `true` on the test
projects. Note this is distinct from `GenerateTestingPlatformEntryPoint`, which stays `false`
because Prova's generator emits the entry point itself.

### 3. `dotnet test` exited 0 even when tests failed

The generated `RunAllAsync` returned `Task`, so the exit code from `app.RunAsync()` and the
failure count from the simple runner were both computed and then discarded. `await someTaskOfInt;`
as a statement compiles cleanly while throwing the value away, which is why this was never
noticed. Any CI consuming Prova would have passed unconditionally.

**Fixed** end-to-end across all four links: `RunMtpAsync` captures the MTP exit code,
`RunSimpleAsync` returns `failed > 0 ? 1 : 0`, `RunAllAsync` returns `Task<int>`, and the emitted
`Program.g.cs` returns it. Verified in both directions: exit 2 with failures, exit 0 when green.

**Test:** `tests/Prova.Generators.Tests/Execution/ExitCodeTests.cs` pins every link.

### 4. The source generator injected an entry point into class libraries

The generator emitted top-level statements whenever it did not find an existing entry point,
rather than checking whether the compilation produces an executable. Class libraries referencing
Prova failed with CS8805.

**Fixed.** The generator now requires `OutputKind.ConsoleApplication` or `WindowsApplication`.

## Severity 2 — documented public APIs that compiled and did nothing

### 5. `HybridMtpAdapter` ignored every concurrency-isolation attribute

The adapter ran all tests through one path that never consulted the isolation attributes, so
`[DoNotParallelize]` and friends were accepted by the compiler and silently dropped. Tests
documented as isolated ran concurrently.

**Fixed.** **Test:** `tests/Prova.Core.Tests/Framework/ConcurrencyIsolationTests.cs`.

### 6. `[BeforeAll]`, `[BeforeEach]`, `[AfterAll]` and `[AfterEach]` never ran

`BeforeAllAttribute` derives from `BeforeAttribute`. `SyntaxAnalyzer` matched hook attributes by
exact type name and never walked the base-type chain, so all four documented alias attributes
compiled and produced empty hook lists.

There is a second trap here: an alias passes its scope to the base constructor
(`BeforeAllAttribute() : base(HookScope.Class)`), so the *applied* attribute has zero constructor
arguments. Scope cannot be read from `ConstructorArguments` and must be resolved from the
attribute type.

**Fixed.** `IsHookAttribute` walks the base-type chain and `GetHookScope` resolves scope from the
attribute type, with the base constructor argument as fallback. Three detection sites rewired.

**Test:** three tests in `tests/Prova.Generators.Tests/Execution/LifecycleHookTests.cs`.

### 7. `[ArgumentDisplayFormatter]` was ignored on `[Matrix]`

The formatter was applied only on the `[DisplayName]` formatting path. The *default* display name
used the raw arguments. The existing `MemberData` test passed only because it happened to also
specify `[DisplayName]`.

**Fixed** in `SourceEmitter.EmitMethodExecution`. The general lesson: when a feature has two
emission paths, test the one users hit by default.

### 8. The migration code fix corrupted line endings

`MigrationCodeFixProvider` hardcoded `SyntaxFactory.EndOfLine("\n")`, so applying the xunit/NUnit
migration fix to a CRLF file left it with mixed line endings. A second, subtler variant: nodes
built with `SyntaxFactory` carry elastic trivia that the formatter later expands with its own
default newline, overriding the explicit trivia.

**Fixed.** The provider now derives the end-of-line from the document being edited and normalises
elastic trivia away, and preserves the surrounding indentation when inserting `[Theory]`.

**Tests:** `Fix_NUnitTestCase_ConvertsToTheoryInlineData` and `Fix_MSTest_FullMigration`.

## Severity 3 — tests that existed but never executed

### 9. `Prova.Generators.Tests` defined 66 tests and executed 11

**Fixed.** All 74 now run, on every target framework.

### 10. `Prova.Analyzers.Tests` was absent from the solution

Thirteen tests covering the migration analyzer and its code fixes had never been built or run.
The project was also pinned to MSTest 3.1.1 plus `Microsoft.NET.Test.Sdk`, which the MTP runner
cannot execute.

**Fixed.** Migrated to MSTest 4.4.1 with `EnableMSTestRunner`, added to `Prova.sln` and `Prova.slnx`.
The deprecated `Microsoft.CodeAnalysis.*.Testing.MSTest` verifier packages had to be replaced with
the framework-agnostic packages and `DefaultVerifier`, because the deprecated ones bind to MSTest
3's `Microsoft.VisualStudio.TestPlatform.TestFramework` assembly, which MSTest 4 no longer ships.
Running them for the first time immediately surfaced defect 8.

### 11. An orphaned test file sat outside any project

`src/Prova.Generators.Tests/Emission/FsCheckEmissionTests.cs` had no `.csproj` and used a
VerifyXunit API the repository does not reference.

**Fixed.** Ported into the real test project and the orphan directory removed. Running it revealed
that the generator-verification compilation did not reference `Prova.FsCheck`, so `[Property]` was
an error type, the generator discovered no tests, and the assertions were being made against an
empty generator run. The reference was added.

### 12. Six sample projects were in no solution and two could not build in Release

`LoggingSample`, `ScriptingSample`, `StateSharingSample`, `VariantSample`, `Prova.MtpSample` and
`Prova.Skugga.Demo` existed on disk but were in neither `Prova.sln` nor `Prova.slnx`. Two of them
referenced the generator through a hardcoded `..\src\Prova.Generators\bin\Debug\netstandard2.0\`
path, so they only built after a Debug build and failed in Release.

**Fixed.** All six added to both solutions; the hardcoded paths replaced with a `ProjectReference`
carrying `OutputItemType="Analyzer"`.

`CrashSample` and `HangSample` are deliberately excluded: they crash and hang by design as
fixtures for the runner's crash/hang dump handling, and would deadlock a solution-wide test run.

## Severity 4 — supply chain, packaging and portability

### 13. High-severity vulnerability in a transitive dependency (NU1903)

`Testcontainers` 4.10.0 pulled `SSH.NET` 2025.1.0, which carries two high-severity advisories
(GHSA-mggc-4xg6-vcxf, GHSA-q939-rpr3-3284).

**Fixed.** `Testcontainers` moved to 4.15.0, which resolves `SSH.NET` 2026.0.0. Restore is clean.

### 14. A shipping library depended on an abandoned preview package

`Prova.AspNetCore` referenced `Microsoft.AspNetCore.Mvc.Testing` 10.0.0-preview.1.25120.3 while
10.0.12 stable existed. It also pinned the package to net10.0.

**Fixed.** Framework-conditional stable references: 8.0.2 for net8.0, 10.0.12 otherwise.

### 15. Package versions drifted across the suite

`Prova.Core` declared 0.5.0 while `Prova.AspNetCore`, `Prova.FsCheck`, `Prova.Playwright` and
`Prova.Testcontainers` each declared 0.4.0, so a release would ship mismatched packages that
reference each other.

**Fixed.** The per-project overrides were removed; all five inherit the repository version.

### 16. `dotnet pack` produced sample and test packages

**Fixed.** `Directory.Build.props` now sets `IsPackable=false` outside `src/`. Verified by
unzipping: five packages, each with `lib/net8.0` and `lib/net10.0`, and the generator and analyzer
shipped under `analyzers/dotnet/cs/`.

### 17. `AnalysisLevel` was `latest`, a forward-compatibility time bomb

`latest` means "whatever the newest SDK on the machine enforces", so installing a new SDK can fail
a build that has not changed. The same setting produced 314 errors elsewhere in the suite on
.NET 11 RC1.

**Fixed.** Pinned to `10.0`.

### 18. `global.json` could not roll forward to a new major SDK

`rollForward: latestFeature` refuses any SDK outside the pinned major, so the repository could not
be built or verified on a .NET 11 preview SDK at all.

**Fixed.** `rollForward: latestMajor` with `allowPrerelease: true`, matching the rest of the suite.

### 19. `.editorconfig` demanded LF but nothing enforced it

The repository had no `.gitattributes`, so Git rewrote checkouts to CRLF on Windows and the
working tree could never satisfy `.editorconfig`. The repository was also not
`dotnet format`-clean: 12,065 whitespace violations.

**Fixed.** Added `.gitattributes` with `* text=auto eol=lf`, formatted the repository, and added a
CI gate. The gate is scoped to `dotnet format whitespace` and `dotnet format style` because the
full `dotnet format` runs analyzers, which needs a workspace compilation, and MSBuildWorkspace
does not run Prova's source generator.

### 20. Prova was net10.0-only

This blocked consumers on the current LTS, and specifically prevented AutoMappic from testing on
net8.0 at all.

**Fixed.** All five shipping libraries and both primary test projects now build and test on
net8.0 and net10.0, with net11.0 available behind `INCLUDE_PREVIEW_TFM=true`.

### 21. CI built one framework, on one OS, and ran two named projects

The workflow targeted net10.0 on ubuntu-latest and invoked `Prova.Core.Tests` and
`Prova.Generators.Tests` by name, so `Prova.Analyzers.Tests` would not have run even once it was
in the solution. There was no coverage, no format check and no preview leg.

**Fixed.** The workflow now runs a Linux/Windows matrix across net8.0 and net10.0, tests the whole
solution, collects coverage, verifies formatting, packs, uploads artifacts, and has an advisory
net11.0 preview leg.

## Open

### 22. The MTP adapter supports no name-based test filtering

`dotnet test --filter-method '*Foo*'` is rejected with *"Unknown option '--filter-method'"* and
exits 5 having run nothing. Only `--filter-uid` is offered, and UIDs are not discoverable without
first listing tests. Investigating a single failure currently means running the whole project and
reading the output.

**Not fixed.** This is a usability gap rather than a correctness bug, but it is the single most
likely thing to frustrate a new contributor.

### 23. `MapToNode` uses `DisplayName` as `TestNodeUid`

Display names are not guaranteed unique, particularly across data-driven rows in different
classes. If two tests produce the same display name their MTP node identities collide. Not yet
proven to be reachable in practice, and recorded here rather than left unmentioned.

### 24. `Prova.Aspire.Sample` is an empty directory

It contains no project file. Either the sample was never written or it was removed without
deleting the folder.

## Verification

| Framework | Result |
| --- | --- |
| net8.0 | `Prova.Core.Tests` and `Prova.Generators.Tests` pass |
| net10.0 | All three test projects pass |
| net11.0 RC1 | 114 tests pass (`Prova.Core.Tests` 40, `Prova.Generators.Tests` 74) |

Solution-wide: **241 tests, 0 failed, 8 skipped, exit code 0**, with `dotnet format whitespace`
and `dotnet format style` both clean.