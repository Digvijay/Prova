using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prova.Analyzers;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Prova.Analyzers.MigrationAnalyzer>;

namespace Prova.Analyzers.Tests
{
    [TestClass]
    public class MigrationAnalyzerTests
    {
        [TestMethod]
        public async Task Analyze_UsingXunit_ReportsDiagnostic()
        {
            var test = @"
using Xunit;

namespace Xunit { public class MockAssert {} }

public class TestClass { }";

            var expected = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticId)
                .WithLocation(2, 1)
                .WithArguments("using Xunit");

            await CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task Analyze_NUnitAttribute_ReportsDiagnostic()
        {
            var test = @"
using Prova;
public class TestClass
{
    [Test]
    public void MyTest() { }
}";

            var expected = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitAttribute)
                .WithLocation(5, 6);

            var analyzerTest = new CSharpAnalyzerTest<MigrationAnalyzer, MSTestVerifier>
            {
                TestCode = test,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            analyzerTest.ExpectedDiagnostics.Add(expected);

            await analyzerTest.RunAsync();
        }

        [TestMethod]
        public async Task Fix_UsingXunit_ReplacesWithUsingProva()
        {
            var test = "using Xunit;\n" +
"\n" +
"namespace Xunit { public class MockAssert {} }\n" +
"namespace Prova { }\n" +
"namespace Prova.Assertions { }\n" +
"\n" +
"public class TestClass { }";

            var fixtest = "using Prova;\n" +
"\n" +
"namespace Xunit { public class MockAssert {} }\n" +
"namespace Prova { }\n" +
"namespace Prova.Assertions { }\n" +
"\n" +
"public class TestClass { }";

            var expected = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticId)
                .WithLocation(1, 1)
                .WithArguments("using Xunit");

            await CSharpCodeFixVerifier<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task Fix_UsingNUnit_ReplacesWithUsingProva()
        {
            var test = "using NUnit.Framework;\n" +
"\n" +
"namespace Prova { }\n" +
"\n" +
"public class TestClass { }";

            var fixtest = "using Prova;\n" +
"\n" +
"namespace Prova { }\n" +
"\n" +
"public class TestClass { }";

            var expected = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitUsing)
                .WithLocation(1, 1);

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            codeFixTest.ExpectedDiagnostics.Add(expected);

            await codeFixTest.RunAsync();
        }

        [TestMethod]
        public async Task Fix_NUnitAttribute_ReplacesWithFact()
        {
            var test = @"
using Prova;
public class TestClass
{
    [Test]
    public void MyTest() { }
}";
            
            var fixtest = @"
using Prova;
public class TestClass
{
    [Fact]
    public void MyTest() { }
}";

            var expected = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitAttribute)
                .WithLocation(5, 6);

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            codeFixTest.ExpectedDiagnostics.Add(expected);

            await codeFixTest.RunAsync();
        }

        [TestMethod]
        public async Task Fix_NUnitTestCase_ConvertsToTheoryInlineData()
        {
            var test = @"
using Prova;
public class TestClass
{
    [TestCase(1)]
    [TestCase(2)]
    public void MyTest(int i) { }
}";
            
            var fixtest = @"
using Prova;
public class TestClass
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void MyTest(int i) { }
}";

            // Expect a diagnostic for each TestCase
            var expected1 = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitTestCase)
                .WithLocation(5, 6);
            var expected2 = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitTestCase)
                .WithLocation(6, 6);

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            // Note: BatchFixer should handle multiple diagnostics
            codeFixTest.ExpectedDiagnostics.Add(expected1);
            codeFixTest.ExpectedDiagnostics.Add(expected2);

            await codeFixTest.RunAsync();
        }

        [TestMethod]
        public async Task Analyze_IClassFixture_ReportsDiagnostic()
        {
            var test = @"
using Xunit;
public class MyTest : IClassFixture<MyFixture> { }";
            
            var expectedUsing = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticId)
                .WithLocation(2, 1);
            var expectedFixture = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdXunitClassFixture)
                .WithLocation(3, 23);

            var analyzerTest = new CSharpAnalyzerTest<MigrationAnalyzer, MSTestVerifier>
            {
                TestCode = test,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            analyzerTest.ExpectedDiagnostics.Add(expectedUsing);
            analyzerTest.ExpectedDiagnostics.Add(expectedFixture);

            await analyzerTest.RunAsync();
        }

        [TestMethod]
        public async Task Fix_IClassFixture_RemovesInterface()
        {
             var test = "using Xunit;\n" +
"public class MyTest : IClassFixture<MyFixture> { }";

            // The code fix provider will also fix the using statement iteratively
            var fixtest = "using Prova;\n" +
"public class MyTest { }";

            var expectedUsing = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticId)
                .WithLocation(1, 1);
            var expectedFixture = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdXunitClassFixture)
                .WithLocation(2, 23);

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                NumberOfFixAllIterations = 2,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            
            // Input has both
            codeFixTest.ExpectedDiagnostics.Add(expectedUsing);
            codeFixTest.ExpectedDiagnostics.Add(expectedFixture);

            await codeFixTest.RunAsync();
        }

        [TestMethod]
        public async Task Fix_OneTimeSetUp_ReplacesWithBeforeAll()
        {
            var test = "using NUnit.Framework;\n" +
"public class MyTest\n" +
"{\n" +
"    [OneTimeSetUp]\n" +
"    public void Setup() { }\n" +
"}";

            var fixtest = "using Prova;\n" +
"public class MyTest\n" +
"{\n" +
"    [BeforeAll]\n" +
"    public void Setup() { }\n" +
"}";

            var expectedUsing = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitUsing)
                .WithLocation(1, 1);
            var expectedSetup = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitOneTimeSetUp)
                .WithLocation(4, 6);

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                NumberOfFixAllIterations = 2,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            
            codeFixTest.ExpectedDiagnostics.Add(expectedUsing);
            codeFixTest.ExpectedDiagnostics.Add(expectedSetup);

            await codeFixTest.RunAsync();
        }

        [TestMethod]
        public async Task Fix_SetUp_ReplacesWithBeforeEach()
        {
            var test = "using NUnit.Framework;\n" +
"public class MyTest\n" +
"{\n" +
"    [SetUp]\n" +
"    public void Setup() { }\n" +
"}";

            var fixtest = "using Prova;\n" +
"public class MyTest\n" +
"{\n" +
"    [BeforeEach]\n" +
"    public void Setup() { }\n" +
"}";

            var expectedUsing = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitUsing)
                .WithLocation(1, 1);
            var expectedSetup = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitSetUp)
                .WithLocation(4, 6);

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                NumberOfFixAllIterations = 2,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            
            codeFixTest.ExpectedDiagnostics.Add(expectedUsing);
            codeFixTest.ExpectedDiagnostics.Add(expectedSetup);

            await codeFixTest.RunAsync();
        }

        [TestMethod]
        public async Task Fix_TearDown_ReplacesWithAfterEach()
        {
            var test = "using NUnit.Framework;\n" +
"public class MyTest\n" +
"{\n" +
"    [TearDown]\n" +
"    public void Teardown() { }\n" +
"}";

            var fixtest = "using Prova;\n" +
"public class MyTest\n" +
"{\n" +
"    [AfterEach]\n" +
"    public void Teardown() { }\n" +
"}";

            var expectedUsing = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitUsing)
                .WithLocation(1, 1);
            var expectedTeardown = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitTearDown)
                .WithLocation(4, 6);

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                NumberOfFixAllIterations = 2,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            
            codeFixTest.ExpectedDiagnostics.Add(expectedUsing);
            codeFixTest.ExpectedDiagnostics.Add(expectedTeardown);

            await codeFixTest.RunAsync();
        }

        [TestMethod]
        public async Task Fix_OneTimeTearDown_ReplacesWithAfterAll()
        {
            var test = "using NUnit.Framework;\n" +
"public class MyTest\n" +
"{\n" +
"    [OneTimeTearDown]\n" +
"    public void Cleanup() { }\n" +
"}";

            var fixtest = "using Prova;\n" +
"public class MyTest\n" +
"{\n" +
"    [AfterAll]\n" +
"    public void Cleanup() { }\n" +
"}";

            var expectedUsing = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitUsing)
                .WithLocation(1, 1);
            var expectedCleanup = CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdNUnitOneTimeTearDown)
                .WithLocation(4, 6);

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                NumberOfFixAllIterations = 2,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            
            codeFixTest.ExpectedDiagnostics.Add(expectedUsing);
            codeFixTest.ExpectedDiagnostics.Add(expectedCleanup);

            await codeFixTest.RunAsync();
        }
        [TestMethod]
        public async Task Fix_MSTest_FullMigration()
        {
            var test = @"using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass]
public class MyTests
{
    [ClassInitialize]
    public static void GlobalInit(TestContext context) { }

    [TestInitialize]
    public void Init() { }

    [TestMethod]
    public void Test1() { }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Test2(int i) { }

    [TestCleanup]
    public void Cleanup() { }

    [ClassCleanup]
    public static void GlobalCleanup() { }
}";

            var fixtest = @"using Prova;

public class MyTests
{
    [BeforeAll]
    public static void GlobalInit(TestContext context) { }

    [BeforeEach]
    public void Init() { }

    [Fact]
    public void Test1() { }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Test2(int i) { }

    [AfterEach]
    public void Cleanup() { }

    [AfterAll]
    public static void GlobalCleanup() { }
}";

            var codeFixTest = new CSharpCodeFixTest<MigrationAnalyzer, MigrationCodeFixProvider, MSTestVerifier>
            {
                TestCode = test,
                FixedCode = fixtest,
                NumberOfFixAllIterations = 9,
                CompilerDiagnostics = CompilerDiagnostics.None
            };
            
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestUsing).WithLocation(1, 1));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestTestClass).WithLocation(2, 2));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestClassInitialize).WithLocation(5, 6));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestTestInitialize).WithLocation(8, 6));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestTestMethod).WithLocation(11, 6));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestDataTestMethod).WithLocation(14, 6));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestDataRow).WithLocation(15, 6));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestDataRow).WithLocation(16, 6));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestTestCleanup).WithLocation(19, 6));
            codeFixTest.ExpectedDiagnostics.Add(CSharpAnalyzerVerifier<MigrationAnalyzer, MSTestVerifier>.Diagnostic(MigrationAnalyzer.DiagnosticIdMSTestClassCleanup).WithLocation(22, 6));

            await codeFixTest.RunAsync();
        }
    }
}
