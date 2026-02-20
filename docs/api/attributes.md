# Attributes Reference 🏷️

Prova uses a set of attributes to mark tests and configure their behavior. These attributes are processed at compile-time by the Prova Source Generator.

## Core Attributes

### `[Fact]`
Marks a method as a test case.
- **Target**: Method
- **Requirements**: Must be `public`. Can return `void` or `Task`.
- **Properties**:
  - `DisplayName`: Optional name to show in reports.
  - `Skip`: Optional reason to skip the test.

### `[Theory]`
Marks a method as a data-driven test case.
- **Target**: Method
- **Data Providers**: Must be accompanied by `[InlineData]`, `[MemberData]`, or `[ClassData]`.

### `[InlineData]`
Provides a single row of data for a `[Theory]`.
- **Arguments**: Match the parameters of the test method.

### `[MemberData]`
Sources data from a static member (property, method, or field).
- **Arguments**: 
  - `string memberName`: Name of the static member.
  - `params object[] parameters`: Optional parameters for method-sourced data.
- **Named Arguments**:
  - `MemberType`: Optional `Type` of the class containing the member.

### `[ClassData]`
Sources data from a class that implements `IEnumerable<object[]>`.
- **Arguments**: `Type classType`.

---

## Metadata & Reporting

### `[DisplayName]`
Overwrites the name of the test in test reports.
- **Target**: Method

### `[Description]`
Adds a text description to the test metadata.
- **Target**: Method

### `[Trait]` / `[Property]`
Adds arbitrary key-value pairs to the test metadata. Useful for filtering.
- **Target**: Method or Class
- **Arguments**: `string key, string value`.

---

## Lifecycle Hooks

### `[Before]` / `[After]`
Executes code before or after test runs.
- **Target**: Method
- **Scope**: Default is `HookScope.Test`. Use `HookScope.Class` or `HookScope.Assembly` for broader scopes.

### `[BeforeEvery]` / `[AfterEvery]`
Executes code before or after **every** test in the project. These are global hooks.
- **Target**: Method (must be static)

### `[BeforeClass]` / `[AfterClass]`
Executes code before or after all tests in a class.
- **Target**: Method (must be static)

### `[BeforeAssembly]` / `[AfterAssembly]`
Executes code before or after all tests in an assembly.
- **Target**: Method (must be static)

---

## Behavior & Concurrency

### `[Parallel]`
Controls execution concurrency for a class or project.
- **Scope**: Class or Assembly.
- **Arguments**: `int max`.

### `[NotInParallel]`
Strict serial execution for tests sharing a resource. Shorthand for `[ParallelLimiter(key, 1)]`.
- **Arguments**: `params string[] keys`.

### `[ParallelLimiter]`
Restricts the number of concurrent tests for a specific resource key.
- **Arguments**: `string key, int limit`.

### `[ParallelGroup]`
Logical grouping for tests, useful for resource allocation and reporting.
- **Arguments**: `string name`.

### `[Sequential]`
Marks a class or assembly as sequential, disabling parallel execution for its children.

---

## Dependency Injection

### `[ConfigureServices]`
Marks a static method used to register services in the `ProvaServiceProvider`.
- **Target**: Method (static)
- **Parameter**: Must accept `ProvaServiceProvider`.

### `[TestDependency]`
Marks a static factory method used as a dependency for test constructors.
- **Target**: Method (static)

### `[ClassFactory]`
Overrides how a test class is instantiated.
- **Arguments**: `Type factoryType` (must implement `IClassConstructor`).

---

## Quality Governance

### `[MaxAlloc]`
Enforces a memory allocation budget per test.
- **Arguments**: `long bytes`.

### `[Timeout]`
Enforces a maximum execution time.
- **Arguments**: `int milliseconds`.

### `[Retry]`
Automatically retries a failing test.
- **Arguments**: `int count`.

### `[Repeat]`
Runs a test multiple times regardless of outcome.
- **Arguments**: `int count`.

### `[Focus]`
Isolates execution. If ANY test has `[Focus]`, all others are ignored.
- **Target**: Method
