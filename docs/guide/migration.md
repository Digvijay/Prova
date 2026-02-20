# Migrating from xUnit

Prova is designed to make migration from xUnit as seamless as possible. While the engines are different, the syntax is intentionally similar.

## Automated Migration

Prova includes a Roslyn Analyzer and Code Fix provider to help you migrate your `using` statements.

### 1. Install Prova
Add the Prova package to your test project.

```xml
<PackageReference Include="Prova" Version="0.4.0" />
```

### 2. Run the Fixer

The Prova Analyzer identifies migration opportunities and provides automated code fixes. You can apply these either via your IDE or through the command line.

#### IDE Experience
The analyzer marks obsolete framework patterns as **Warnings**. Use the standard "Quick Fix" shortcut to migrate:
- **Visual Studio**: `Ctrl + .`
- **VS Code**: `Cmd + .` (Mac) or `Ctrl + .` (Windows)

#### Bulk Migration via CLI
To migrate an entire project or solution at once, use the `dotnet format` command. This is the fastest way to handle large codebases.

```bash
# Apply all Prova migration fixes to the solution
dotnet format analyzers --severity warn --diagnostics PRV001 PRV002 PRV003 PRV004 PRV005 PRV006 PRV007 PRV008 PRV009 PRV010 PRV011 PRV012 PRV013 PRV014 PRV015 PRV016 PRV017 PRV018
```

##### Command Parameters
- `--severity warn`: Fixes diagnostics marked as warnings (Prova defaults to warn for migration).
- `--diagnostics <IDS>`: Filters by specific Prova diagnostic IDs (see table below).
- `analyzers`: Tells dotnet-format to run analyzer-based code fixes instead of just whitespace formatting.

##### Prova Diagnostic IDs
| ID | Framework | Target | Prova Equivalent |
|----|-----------|--------|------------------|
| **PRV001** | xUnit | `using Xunit;` | `using Prova;` |
| **PRV002** | NUnit | `using NUnit.Framework;` | `using Prova;` |
| **PRV003** | NUnit | `[Test]` | `[Fact]` |
| **PRV004** | NUnit | `[TestCase]` | `[Theory]` + `[InlineData]` |
| **PRV005** | xUnit | `IClassFixture<T>` | Class Constructor |
| **PRV006** | NUnit | `[OneTimeSetUp]` | `[BeforeAll]` |
| **PRV007** | NUnit | `[SetUp]` | `[BeforeEach]` |
| **PRV008** | NUnit | `[TearDown]` | `[AfterEach]` |
| **PRV009** | NUnit | `[OneTimeTearDown]` | `[AfterAll]` |
| **PRV010** | MSTest | `using UnitTesting;` | `using Prova;` |
| **PRV011** | MSTest | `[TestMethod]` | `[Fact]` |
| **PRV012** | MSTest | `[DataTestMethod]` | `[Theory]` |
| **PRV013** | MSTest | `[DataRow]` | `[InlineData]` |
| **PRV014** | MSTest | `[TestInitialize]` | `[BeforeEach]` |
| **PRV015** | MSTest | `[TestCleanup]` | `[AfterEach]` |
| **PRV016** | MSTest | `[ClassInitialize]` | `[BeforeAll]` |
| **PRV017** | MSTest | `[ClassCleanup]` | `[AfterAll]` |
| **PRV018** | MSTest | `[TestClass]` | *Removed* |

> [!NOTE]
> Ensure you have built the project at least once so the analyzer can see the dependencies.

### Automated Transformations

Below is a detailed breakdown of what the fixer handles for each framework:

#### xUnit
- **Imports**: Replaces `using Xunit;` with `using Prova;`.
- **DI**: Detects and removes `IClassFixture<T>` (Prova uses native Constructor Injection).

#### NUnit
- **Imports**: Replaces `using NUnit.Framework;` with `using Prova;`.
- **Attributes**:
    - `[Test]` → `[Fact]`
    - `[TestCase(args)]` → `[InlineData(args)]` (also adds/replaces with `[Theory]`).
- **Lifecycle**:
    - `[OneTimeSetUp]` → `[BeforeAll]`
    - `[SetUp]` → `[BeforeEach]`
    - `[TearDown]` → `[AfterEach]`
    - `[OneTimeTearDown]` → `[AfterAll]`

#### MSTest
- **Imports**: Replaces `using Microsoft.VisualStudio.TestTools.UnitTesting;` with `using Prova;`.
- **Attributes**:
    - `[TestMethod]` → `[Fact]`
    - `[DataTestMethod]` → `[Theory]`
    - `[DataRow(args)]` → `[InlineData(args)]`
    - `[TestClass]` → Removed (not required by Prova).
- **Lifecycle**:
    - `[TestInitialize]` → `[BeforeEach]`
    - `[TestCleanup]` → `[AfterEach]`
    - `[ClassInitialize]` → `[BeforeAll]`
    - `[ClassCleanup]` → `[AfterAll]`

### Attributes
Most attributes map 1:1, but verify the following:

| xUnit | NUnit | MSTest | Prova | Notes |
|-------|-------|--------|-------|-------|
| `[Fact]` | `[Test]` | `[TestMethod]` | `[Fact]` | Use Analyzer to fix |
| `[Theory]` | `[TestCase]` | `[DataTestMethod]` | `[Theory]` | Use Analyzer to fix |
| `[InlineData]` | - | `[DataRow]` | `[InlineData]` | Use Analyzer to fix |
| `IClassFixture<T>` | - | - | Class Constructor | Use Analyzer to fix (removes interface) |
| - | `[OneTimeSetUp]` | `[ClassInitialize]` | `[BeforeAll]` | Use Analyzer to fix |
| - | `[OneTimeTearDown]` | `[ClassCleanup]` | `[AfterAll]` | Use Analyzer to fix |
| `[Setup]` / `[Before]` | `[SetUp]` | `[TestInitialize]` | `[BeforeEach]` | Use Analyzer to fix |
| `[TearDown]` / `[After]` | `[TearDown]` | `[TestCleanup]` | `[AfterEach]` | Use Analyzer to fix |
| `ICollectionFixture<T>` | `[TestDependency]` | - | `[TestDependency]` | Use specialized lifecycle attributes |

## Migration Matrix Examples

Prova's automated migration covers major patterns across xUnit, NUnit, and MSTest. Below are detailed examples of each transformation.

### 1. Using Directive (xUnit & NUnit)
The analyzer detects framework imports and replaces them with Prova.

````carousel
```csharp
// Before
using Xunit;
using NUnit.Framework;

// After
using Prova;
```
````

### 2. Basic Tests (NUnit `[Test]`)
Converts NUnit's `[Test]` to Prova's `[Fact]`.

````carousel
```csharp
// Before (NUnit)
[Test]
public void MyTest() { }

// After (Prova)
[Fact]
public void MyTest() { }
```
````

### 3. Parameterized Tests (NUnit `[TestCase]`)
NUnit's `[TestCase]` is converted to a combination of `[Theory]` and `[InlineData]`.

````carousel
```csharp
// Before (NUnit)
[TestCase(1, "A")]
[TestCase(2, "B")]
public void MyTest(int id, string name) { }

// After (Prova)
[Theory]
[InlineData(1, "A")]
[InlineData(2, "B")]
public void MyTest(int id, string name) { }
```
````

### 4. Dependency Injection (xUnit `IClassFixture<T>`)
Removes the `IClassFixture` interface; Prova handles constructor injection natively.

````carousel
```csharp
// Before (xUnit)
public class Tests : IClassFixture<MyFixture>
{
    public Tests(MyFixture fixture) { }
}

// After (Prova)
public class Tests
{
    public Tests(MyFixture fixture) { }
}
```
````

### 5. Global Setup (NUnit `[OneTimeSetUp]`)
Maps NUnit's class-level setup to `[BeforeAll]`.

````carousel
```csharp
// Before (NUnit)
[OneTimeSetUp]
public void Init() { }

// After (Prova)
[BeforeAll]
public void Init() { }
```
````

### 6. Test Setup (NUnit `[SetUp]`)
Maps individual test setup to `[BeforeEach]`.

````carousel
```csharp
// Before (NUnit)
[SetUp]
public void Setup() { }

// After (Prova)
[BeforeEach]
public void Setup() { }
```
````

### 7. Test Teardown (NUnit `[TearDown]`)
Maps individual test teardown to `[AfterEach]`.

````carousel
```csharp
// Before (NUnit)
[TearDown]
public void Teardown() { }

// After (Prova)
[AfterEach]
public void Teardown() { }
```
````

### 8. Global Teardown (NUnit `[OneTimeTearDown]`)
Maps class-level finalization to `[AfterAll]`.

````carousel
```csharp
// Before (NUnit)
[OneTimeTearDown]
public void Cleanup() { }

// After (Prova)
[AfterAll]
public void Cleanup() { }
```
````

### 9. Assertions
While not yet automated by the analyzer, Prova provides a 1:1 mapping for most frameworks.

````carousel
```csharp
// xUnit / NUnit / MSTest
Assert.Equal(a, b);    // xUnit / Prova
Assert.AreEqual(a, b); // MSTest (use Equal)
Assert.True(condition);

// Prova
Assert.Equal(a, b);
Assert.True(condition);
```
````

### 10. MSTest Lifecycle
MSTest `[TestInitialize]` and `[TestCleanup]` are mapped to `[BeforeEach]` and `[AfterEach]`.

````carousel
```csharp
// Before (MSTest)
[TestClass]
public class MyTests
{
    [TestInitialize]
    public void Init() { }
}

// After (Prova)
public class MyTests
{
    [BeforeEach]
    public void Init() { }
}
```
````
