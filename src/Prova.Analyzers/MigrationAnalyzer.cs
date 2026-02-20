using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Prova.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MigrationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PRV001";
        public const string DiagnosticIdNUnitUsing = "PRV002";
        public const string DiagnosticIdNUnitAttribute = "PRV003";
        public const string DiagnosticIdNUnitTestCase = "PRV004";
        public const string DiagnosticIdXunitClassFixture = "PRV005";
        public const string DiagnosticIdNUnitOneTimeSetUp = "PRV006";
        public const string DiagnosticIdNUnitSetUp = "PRV007";
        public const string DiagnosticIdNUnitTearDown = "PRV008";
        public const string DiagnosticIdNUnitOneTimeTearDown = "PRV009";
        public const string DiagnosticIdMSTestUsing = "PRV010";
        public const string DiagnosticIdMSTestTestMethod = "PRV011";
        public const string DiagnosticIdMSTestDataTestMethod = "PRV012";
        public const string DiagnosticIdMSTestDataRow = "PRV013";
        public const string DiagnosticIdMSTestTestInitialize = "PRV014";
        public const string DiagnosticIdMSTestTestCleanup = "PRV015";
        public const string DiagnosticIdMSTestClassInitialize = "PRV016";
        public const string DiagnosticIdMSTestClassCleanup = "PRV017";
        public const string DiagnosticIdMSTestTestClass = "PRV018";
        public const string DiagnosticIdTestContext = "PRV019";

        private const string Title = "Migrate to Prova";
        private const string MessageFormat = "Use 'using Prova' instead of 'using Xunit'";
        private const string MessageFormat_NUnitUsing = "Use 'using Prova' instead of 'using NUnit.Framework'";
        private const string MessageFormat_NUnitAttribute = "Use '[Fact]' instead of '[Test]'";
        private const string MessageFormat_NUnitTestCase = "Use '[InlineData]' with '[Theory]' instead of '[TestCase]'";
        private const string MessageFormat_XunitClassFixture = "Remove 'IClassFixture<T>'. Prova supports Constructor Injection natively.";
        private const string MessageFormat_NUnitOneTimeSetUp = "Use '[BeforeClass]' instead of '[OneTimeSetUp]'";
        private const string MessageFormat_NUnitSetUp = "Use '[Before]' instead of '[SetUp]'";
        private const string MessageFormat_NUnitTearDown = "Use '[After]' instead of '[TearDown]'";
        private const string MessageFormat_NUnitOneTimeTearDown = "Use '[AfterClass]' instead of '[OneTimeTearDown]'";
        private const string MessageFormat_MSTestUsing = "Use 'using Prova' instead of 'using Microsoft.VisualStudio.TestTools.UnitTesting'";
        private const string MessageFormat_MSTestTestMethod = "Use '[Fact]' instead of '[TestMethod]'";
        private const string MessageFormat_MSTestDataTestMethod = "Use '[Theory]' instead of '[DataTestMethod]'";
        private const string MessageFormat_MSTestDataRow = "Use '[InlineData]' instead of '[DataRow]'";
        private const string MessageFormat_MSTestTestInitialize = "Use '[Before]' instead of '[TestInitialize]'";
        private const string MessageFormat_MSTestTestCleanup = "Use '[After]' instead of '[TestCleanup]'";
        private const string MessageFormat_MSTestClassInitialize = "Use '[BeforeClass]' instead of '[ClassInitialize]'";
        private const string MessageFormat_MSTestClassCleanup = "Use '[AfterClass]' instead of '[ClassCleanup]'";
        private const string MessageFormat_MSTestTestClass = "Remove '[TestClass]'. Prova does not require this attribute.";
        private const string MessageFormat_TestContext = "Use 'Prova.TestContext.Current' instead of injecting 'TestContext'";
        
        private const string Description = "Prova provides better performance and reliability.";
        private const string Category = "Migration";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleNUnitUsing = new DiagnosticDescriptor(DiagnosticIdNUnitUsing, Title, MessageFormat_NUnitUsing, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleNUnitAttribute = new DiagnosticDescriptor(DiagnosticIdNUnitAttribute, Title, MessageFormat_NUnitAttribute, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleNUnitTestCase = new DiagnosticDescriptor(DiagnosticIdNUnitTestCase, Title, MessageFormat_NUnitTestCase, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleXunitClassFixture = new DiagnosticDescriptor(DiagnosticIdXunitClassFixture, Title, MessageFormat_XunitClassFixture, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleNUnitOneTimeSetUp = new DiagnosticDescriptor(DiagnosticIdNUnitOneTimeSetUp, Title, MessageFormat_NUnitOneTimeSetUp, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleNUnitSetUp = new DiagnosticDescriptor(DiagnosticIdNUnitSetUp, Title, MessageFormat_NUnitSetUp, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleNUnitTearDown = new DiagnosticDescriptor(DiagnosticIdNUnitTearDown, Title, MessageFormat_NUnitTearDown, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleNUnitOneTimeTearDown = new DiagnosticDescriptor(DiagnosticIdNUnitOneTimeTearDown, Title, MessageFormat_NUnitOneTimeTearDown, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestUsing = new DiagnosticDescriptor(DiagnosticIdMSTestUsing, Title, MessageFormat_MSTestUsing, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestTestMethod = new DiagnosticDescriptor(DiagnosticIdMSTestTestMethod, Title, MessageFormat_MSTestTestMethod, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestDataTestMethod = new DiagnosticDescriptor(DiagnosticIdMSTestDataTestMethod, Title, MessageFormat_MSTestDataTestMethod, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestDataRow = new DiagnosticDescriptor(DiagnosticIdMSTestDataRow, Title, MessageFormat_MSTestDataRow, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestTestInitialize = new DiagnosticDescriptor(DiagnosticIdMSTestTestInitialize, Title, MessageFormat_MSTestTestInitialize, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestTestCleanup = new DiagnosticDescriptor(DiagnosticIdMSTestTestCleanup, Title, MessageFormat_MSTestTestCleanup, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestClassInitialize = new DiagnosticDescriptor(DiagnosticIdMSTestClassInitialize, Title, MessageFormat_MSTestClassInitialize, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestClassCleanup = new DiagnosticDescriptor(DiagnosticIdMSTestClassCleanup, Title, MessageFormat_MSTestClassCleanup, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleMSTestTestClass = new DiagnosticDescriptor(DiagnosticIdMSTestTestClass, Title, MessageFormat_MSTestTestClass, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor RuleTestContext = new DiagnosticDescriptor(DiagnosticIdTestContext, Title, MessageFormat_TestContext, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            Rule, RuleNUnitUsing, RuleNUnitAttribute, RuleNUnitTestCase, RuleXunitClassFixture, 
            RuleNUnitOneTimeSetUp, RuleNUnitSetUp, RuleNUnitTearDown, RuleNUnitOneTimeTearDown,
            RuleMSTestUsing, RuleMSTestTestMethod, RuleMSTestDataTestMethod, RuleMSTestDataRow,
            RuleMSTestTestInitialize, RuleMSTestTestCleanup, RuleMSTestClassInitialize, RuleMSTestClassCleanup, RuleMSTestTestClass, RuleTestContext);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
            context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeAction(AnalyzeBaseList, SyntaxKind.BaseList);
            context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            var typeName = property.Type.ToString();
            if (typeName == "TestContext" || typeName.EndsWith(".TestContext", global::System.StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleTestContext, property.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
        {
            var usingDirective = (UsingDirectiveSyntax)context.Node;
            if (usingDirective.Name == null) return;
            var name = usingDirective.Name.ToString();
            
            // Check if it is "using Xunit;" or "using Xunit.*"
            if (name.StartsWith("Xunit", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Check for NUnit
            else if (name.Equals("NUnit.Framework", StringComparison.Ordinal) || name.StartsWith("NUnit.Framework.", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleNUnitUsing, usingDirective.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Check for MSTest
            else if (name.Equals("Microsoft.VisualStudio.TestTools.UnitTesting", StringComparison.Ordinal) || name.StartsWith("Microsoft.VisualStudio.TestTools.UnitTesting.", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestUsing, usingDirective.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            var name = attribute.Name.ToString();

            // Check for [Test]
            if (name == "Test" || name == "TestAttribute" || name.EndsWith(".Test", StringComparison.Ordinal) || name.EndsWith(".TestAttribute", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleNUnitAttribute, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Check for [TestCase]
            else if (name == "TestCase" || name == "TestCaseAttribute" || name.EndsWith(".TestCase", StringComparison.Ordinal) || name.EndsWith(".TestCaseAttribute", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleNUnitTestCase, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Check for [OneTimeSetUp]
            else if (name == "OneTimeSetUp" || name == "OneTimeSetUpAttribute" || name.EndsWith(".OneTimeSetUp", StringComparison.Ordinal) || name.EndsWith(".OneTimeSetUpAttribute", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleNUnitOneTimeSetUp, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Check for [SetUp]
            else if (name == "SetUp" || name == "SetUpAttribute" || name.EndsWith(".SetUp", StringComparison.Ordinal) || name.EndsWith(".SetUpAttribute", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleNUnitSetUp, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Check for [TearDown]
            else if (name == "TearDown" || name == "TearDownAttribute" || name.EndsWith(".TearDown", StringComparison.Ordinal) || name.EndsWith(".TearDownAttribute", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleNUnitTearDown, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Check for [OneTimeTearDown]
            else if (name == "OneTimeTearDown" || name == "OneTimeTearDownAttribute" || name.EndsWith(".OneTimeTearDown", StringComparison.Ordinal) || name.EndsWith(".OneTimeTearDownAttribute", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleNUnitOneTimeTearDown, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Check for MSTest attributes
            else if (name == "TestMethod" || name == "TestMethodAttribute" || name.EndsWith(".TestMethod", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestTestMethod, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            else if (name == "DataTestMethod" || name == "DataTestMethodAttribute" || name.EndsWith(".DataTestMethod", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestDataTestMethod, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            else if (name == "DataRow" || name == "DataRowAttribute" || name.EndsWith(".DataRow", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestDataRow, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            else if (name == "TestInitialize" || name == "TestInitializeAttribute" || name.EndsWith(".TestInitialize", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestTestInitialize, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            else if (name == "TestCleanup" || name == "TestCleanupAttribute" || name.EndsWith(".TestCleanup", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestTestCleanup, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            else if (name == "ClassInitialize" || name == "ClassInitializeAttribute" || name.EndsWith(".ClassInitialize", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestClassInitialize, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            else if (name == "ClassCleanup" || name == "ClassCleanupAttribute" || name.EndsWith(".ClassCleanup", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestClassCleanup, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            else if (name == "TestClass" || name == "TestClassAttribute" || name.EndsWith(".TestClass", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(RuleMSTestTestClass, attribute.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeBaseList(SyntaxNodeAnalysisContext context)
        {
            var baseList = (BaseListSyntax)context.Node;
            foreach (var baseType in baseList.Types)
            {
                var name = baseType.Type.ToString();
                if (name.StartsWith("IClassFixture", StringComparison.Ordinal) || name.EndsWith(".IClassFixture", StringComparison.Ordinal))
                {
                    var diagnostic = Diagnostic.Create(RuleXunitClassFixture, baseType.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
