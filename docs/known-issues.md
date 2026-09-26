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

## Severity 5 — usability, identity and the audit boundary

### 22. The MTP adapter supported no name-based test filtering

`dotnet test --filter-method '*Foo*'` was rejected with *"Unknown option '--filter-method'"* and
exited 5 having run nothing. Only `--filter-uid` was offered, and a UID is not discoverable without
first listing the tests. Investigating a single failure meant running the whole project and reading
the output.

**Fixed.** `ProvaFilterCommandLineProvider` adds `--filter-name`, `--filter-class` and
`--filter-method`. Patterns support `*` and `?`; a pattern with no wildcard matches as a substring,
which is what someone typing a bare method name expects. Repeating an option is a disjunction,
combining different options is a conjunction, and every pattern is escaped so that the brackets and
parentheses that appear in data-row names are matched literally rather than reinterpreted as a
regular expression.

The filter is applied on **both** emission paths — the platform host and the standalone runner.
Prova has previously shipped a feature on one path and not the other (defect 7), so the matching
code is shared rather than written twice. On the standalone path the filter's *value* is also
removed from the argument list, because that runner treats any bare argument as a keyword filter and
would otherwise narrow the run a second time, by a different rule, without saying so.

The options appear in `--help` and `--info` alongside the platform's own.

### 23. `MapToNode` used `DisplayName` as `TestNodeUid`

Recorded previously as "not yet proven to be reachable in practice". It is reachable trivially: a
`[DisplayName]` containing no format placeholders produces `string.Format("Adds numbers", 1, 2)` —
the same string for every row of a `[Theory]`. Every row was therefore reported under one node
identity and the rows collapsed into a single node, so a failing row could be reported as passing
because a later row overwrote its result. Editors and CI systems key a test's history off this
value.

**Fixed.** `ProvaTest.UniqueName` now carries a structural name that the generator always emits —
class, method and the default row suffix — independently of any `[DisplayName]`. A label the user is
free to make ambiguous must never be a test's identity. The adapter assigns node identities from
that name and, for registrations built by hand or by an older generator, disambiguates genuine
collisions with a deterministic `#2`, `#3` ordinal rather than letting two tests share one
identifier.

### 24. `Prova.Aspire.Sample` was recorded as an empty directory

**This entry was wrong, and the way it was wrong matters.** The directory is not empty: it holds
three git-tracked projects — an Aspire app host, a service-defaults library and a test project.
They were in no solution, so nothing compiled them and nobody looked at them.

Bringing them into the solution immediately surfaced what had been hidden: a missing
`Aspire.AppHost.Sdk` import, duplicated `<Nullable>` elements, Aspire 9.0.0 against the 13.x used
elsewhere in the programme, `TreatWarningsAsErrors` switched off, `IsTestingPlatformApplication`
inverted — defect 2, recurring — and, on restore, **NU1902 advisories on `KubernetesClient` and
`OpenTelemetry.Api`**.

That last point is the general lesson, and it is sharper than the one recorded earlier in this
document:

> A project outside the build graph is also outside the audit.

The claim that restore was clean across the repository had been true only of projects a solution
knew about. It is now true of the repository.

**Fixed.** All three projects updated to Aspire 13.5.4, OpenTelemetry 1.19.0 and Extensions 10.10.0,
ASPIRE004 and ASPIRE010 resolved explicitly rather than suppressed wholesale, and all three added to
both solution files. The test project keeps `IsTestingPlatformApplication=false` deliberately — it
needs a container runtime — but is now compiled and audited on every build.

### 25. Two solution files drifted apart, and two more projects were in neither

`Prova.sln` and `Prova.slnx` both exist and nothing kept them in step. The `.slnx` was three
projects behind. Worse, `CrashSample` and `HangSample` were in *neither* file.

Both samples had been broken for some time: each declared `public static async Task Main` and then
wrote `return await ...`, which is `CS1997`. **They did not compile.** Nobody knew, because nothing
built them.

**Fixed.** Both samples compile again, target the shared framework set, and are members of both
solutions with `IsTestingPlatformApplication=false` so that `dotnet test` does not try to run a
sample whose entire purpose is to crash or to hang. `SolutionParityTests` now asserts that the two
solution files list the same projects and that no project on disk is absent from a solution — the
check that found these two, and the check that stops the next one.

### 26. `Assert.Equal` compared collections by reference

`Assert.Equal(new[] { 1, 2 }, new[] { 1, 2 })` **failed**. The assertion delegated to
`EqualityComparer<T>.Default`, which compares arrays and lists by reference. Every other .NET test
framework compares sequences element by element. An assertion that fails on equal input is worse
than no assertion at all, because it teaches people to distrust the failure rather than the code.

There was also no `Assert.NotEqual` at all, which pushed people towards `Assert.False(a == b)` — an
assertion whose failure message says nothing about what the values actually were.

**Fixed.** `Assert.Equal` compares collections element by element, recursing into nested ones, while
still treating strings as strings. `Assert.NotEqual` is its exact opposite rather than a second,
subtly different notion of equality. Failure messages now render collection contents, so a message
reads `Expected: [1, 2]` rather than `Expected: System.Int32[]`.

### 27. The adapter reported a hardcoded, stale version

`HybridMtpAdapter.Version` returned the literal `"0.5.0"` while the package was at 0.6.0.

**Fixed.** The version is read from assembly metadata. Anything a human has to remember to update
twice eventually disagrees with itself.

### 28. `--info` was not routed to the platform host

The generated entry point decides between the platform host and the standalone runner by inspecting
the arguments. `--info` was missing from that list, so asking the runner to describe itself silently
ran the entire test suite instead.

**Fixed.** `--info` now routes to the platform host, where it reports Prova's own extension and
version alongside the platform's.

### 29. Every Microsoft.Testing.Platform extension other than the dump providers was ignored

Prova generates its own entry point, so the one Microsoft.Testing.Platform would have generated is
disabled. That generated entry point is where the platform registers the extensions a project
references (code coverage, TRX reports, retry). Prova's replacement registered only the crash-dump
and hang-dump providers by hand. Referencing any other extension had no effect: its command-line
options were rejected as unknown, and the run exited with code 5 having executed zero tests.

Prova also removed `--coverage` from the arguments to drive its own LCOV output, so even a
correctly registered coverage extension would never have seen the switch.

Found by running the CI workflow's own test command locally before pushing it: that command could
not have passed on any runner.

**Fixed.** The generator detects the `SelfRegisteredExtensions` class that
`Microsoft.Testing.Platform.MSBuild` generates and calls it, so every referenced extension is
registered the way the platform intends. It falls back to the dump providers when that class is
absent. `--coverage` is left to the platform extension whenever the extension is referenced. Six
tests in `PlatformExtensionTests` pin both paths.

### 30. The CI coverage step could not run

The workflow collected coverage with `--collect:"XPlat Code Coverage"`, a VSTest data-collector
switch. Microsoft.Testing.Platform rejects it, so the test step would have failed on its first run
with zero tests executed.

**Fixed.** CI uses `--coverage --coverage-output-format cobertura`, and each test project
references `Microsoft.Testing.Extensions.CodeCoverage`. That fix depended on defect 29.

### 31. A sample demonstrated nothing, and two did not build cleanly

`samples/VariantSample` exists to show `[TestVariant]`, but both attributes and both assertions
were commented out, and its second test had no `[Fact]`, so it never ran. The sample passed while
demonstrating no variant behaviour at all. Separately, the .NET 11 RC1 build reported `CA1050` in
`VariantSample` and `LoggingSample` (types in the global namespace) and `CS0162` in
`ScriptingSample`, whose "Script Completed" line followed a `return` and could never print.

**Fixed.** The variant sample runs its test once per variant and asserts the variant name: three
tests pass, where one did before. Both samples declare a namespace, and the scripting sample
returns its exit code after printing. The solution builds with zero warnings on SDK 10 and RC1.

## Open

Nothing is open in Prova.

## Verification

Run with the .NET 11 RC1 SDK (`11.0.100-rc.1.26425.128`) and the preview framework enabled, so
every framework below was built and tested in one pass:

| Framework | `Prova.Core.Tests` | `Prova.Generators.Tests` | `Prova.Analyzers.Tests` |
| --- | --- | --- | --- |
| net8.0 | 66 passed, 4 skipped | 83 passed | — |
| net10.0 | 66 passed, 4 skipped | 83 passed | 13 passed |
| net11.0 RC1 | 66 passed, 4 skipped | 83 passed | — |

Solution-wide: **460 passed, 0 failed, 12 skipped, exit code 0**. The four skips per framework are
deliberate: three are tests written to fail, kept to confirm by hand that failures are reported,
and one exercises skip reporting itself.
