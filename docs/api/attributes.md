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
Sours data from a static member (property, method, or field).
- **Arguments**: 
  - `string memberName`: Name of the static member.
  - `params object[] parameters`: Optional parameters for method-sourced data.
- **Named Arguments**:
  - `MemberType`: Optional `Type` of the class containing the member.

### `[ClassData]`
Sources data from a class that implements `IEnumerable<object[]>`.
- **Arguments**: `Type classType`.

---

## Lifecycle Hooks

### `[Before]`
Executes code before test runs.
- **Target**: Method
- **Scope**: Default is `HookScope.Test`. Use `HookScope.Class` or `HookScope.Assembly` for broader scopes.
- **Requirements**: `Class` and `Assembly` hooks must be `static`.

### `[After]`
Executes code after test runs.
- **Target**: Method
- **Scope**: Default is `HookScope.Test`. Use `HookScope.Class` or `HookScope.Assembly` for broader scopes.
- **Requirements**: `Class` and `Assembly` hooks must be `static`.

---

## Behavior Configuration

### `[Parallel]`
Controls execution concurrency for a class.
- **Target**: Class
- **Arguments**: `int max` (e.g., `[Parallel(2)]`).

### `[NotInParallel]`
Enforces sequential execution for tests sharing a resource.
- **Target**: Method or Class
- **Arguments**: `params string[] keys` (e.g., `[NotInParallel("Database")]`).
- **Effect**: Tests with the same key will NOT run concurrently with each other.

### `[MaxAlloc]`
Enforces a memory allocation budget using `GC.GetAllocatedBytesForCurrentThread()`.
- **Target**: Method
- **Arguments**: `long bytes` (e.g., `[MaxAlloc(1024)]`).

### `[Timeout]`
Enforces a maximum execution time.
- **Target**: Method
- **Arguments**: `int milliseconds` (e.g., `[Timeout(500)]`).

### `[Retry]`
Automatically retries a test if it fails.
- **Target**: Method
- **Arguments**: `int count` (e.g., `[Retry(3)]`).

### `[Focus]`
Isolates execution. If ANY test has `[Focus]`, only those tests will run. All others are skipped.
- **Target**: Method

### `[TestDependency]`
Marks a static factory method for Dependency Injection.
- **Target**: Method (Static)
