using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Prova.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MigrationCodeFixProvider)), Shared]
    public class MigrationCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            MigrationAnalyzer.DiagnosticId,
            MigrationAnalyzer.DiagnosticIdNUnitUsing,
            MigrationAnalyzer.DiagnosticIdNUnitAttribute,
            MigrationAnalyzer.DiagnosticIdNUnitTestCase,
            MigrationAnalyzer.DiagnosticIdXunitClassFixture,
            MigrationAnalyzer.DiagnosticIdNUnitOneTimeSetUp,
            MigrationAnalyzer.DiagnosticIdNUnitSetUp,
            MigrationAnalyzer.DiagnosticIdNUnitTearDown,
            MigrationAnalyzer.DiagnosticIdNUnitOneTimeTearDown,
            MigrationAnalyzer.DiagnosticIdMSTestUsing,
            MigrationAnalyzer.DiagnosticIdMSTestTestMethod,
            MigrationAnalyzer.DiagnosticIdMSTestDataTestMethod,
            MigrationAnalyzer.DiagnosticIdMSTestDataRow,
            MigrationAnalyzer.DiagnosticIdMSTestTestInitialize,
            MigrationAnalyzer.DiagnosticIdMSTestTestCleanup,
            MigrationAnalyzer.DiagnosticIdMSTestClassInitialize,
            MigrationAnalyzer.DiagnosticIdMSTestClassCleanup,
            MigrationAnalyzer.DiagnosticIdMSTestTestClass);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null) return;

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var diagnosticId = diagnostic.Id;

            var node = root.FindNode(diagnosticSpan);
            if (node == null) return;

            if (diagnosticId == MigrationAnalyzer.DiagnosticId || 
                diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitUsing ||
                diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestUsing)
            {
                var usingDirective = node.AncestorsAndSelf().OfType<UsingDirectiveSyntax>().FirstOrDefault();
                if (usingDirective != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Migrate to Prova",
                            createChangedDocument: c => MigrateUsingToProvaAsync(context.Document, usingDirective, c),
                            equivalenceKey: "MigrateUsingToProva"),
                        diagnostic);
                }
            }
            else if (diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitAttribute || 
                     diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitOneTimeSetUp ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitSetUp ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitTearDown ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitOneTimeTearDown ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestTestMethod ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestDataTestMethod ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestDataRow ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestTestInitialize ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestTestCleanup ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestClassInitialize ||
                     diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestClassCleanup)
            {
                var attribute = node.AncestorsAndSelf().OfType<AttributeSyntax>().FirstOrDefault();
                if (attribute != null)
                {
                    string networkName;
                    string title;
                    string eqKey;

                    switch (diagnosticId)
                    {
                        case MigrationAnalyzer.DiagnosticIdNUnitAttribute:
                        case MigrationAnalyzer.DiagnosticIdMSTestTestMethod:
                            networkName = "Fact";
                            title = diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitAttribute ? "Replace with [Fact]" : "Replace [TestMethod] with [Fact]";
                            eqKey = "MigrateAttributeToProva";
                            break;
                        case MigrationAnalyzer.DiagnosticIdMSTestDataTestMethod:
                            networkName = "Theory";
                            title = "Replace [DataTestMethod] with [Theory]";
                            eqKey = "MigrateDataTestMethodToProva";
                            break;
                        case MigrationAnalyzer.DiagnosticIdMSTestDataRow:
                            networkName = "InlineData";
                            title = "Replace [DataRow] with [InlineData]";
                            eqKey = "MigrateDataRowToProva";
                            break;
                        case MigrationAnalyzer.DiagnosticIdNUnitOneTimeSetUp:
                        case MigrationAnalyzer.DiagnosticIdMSTestClassInitialize:
                            networkName = "BeforeAll";
                            title = diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitOneTimeSetUp ? "Replace with [BeforeAll]" : "Replace [ClassInitialize] with [BeforeAll]";
                            eqKey = "MigrateOneTimeSetUpToProva";
                            break;
                        case MigrationAnalyzer.DiagnosticIdMSTestClassCleanup:
                        case MigrationAnalyzer.DiagnosticIdNUnitOneTimeTearDown:
                            networkName = "AfterAll";
                            title = "Replace with [AfterAll]";
                            eqKey = "MigrateOneTimeTearDownToProva";
                            break;
                        case MigrationAnalyzer.DiagnosticIdNUnitSetUp:
                        case MigrationAnalyzer.DiagnosticIdMSTestTestInitialize:
                            networkName = "BeforeEach";
                            title = "Replace with [BeforeEach]";
                            eqKey = "MigrateSetUpToProva";
                            break;
                        case MigrationAnalyzer.DiagnosticIdNUnitTearDown:
                        case MigrationAnalyzer.DiagnosticIdMSTestTestCleanup:
                            networkName = "AfterEach";
                            title = "Replace with [AfterEach]";
                            eqKey = "MigrateTearDownToProva";
                            break;
                        default:
                            return;
                    }

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: title,
                            createChangedDocument: c => ReplaceAttributeAsync(context.Document, attribute, networkName, c),
                            equivalenceKey: eqKey),
                        diagnostic);
                }
            }
            else if (diagnosticId == MigrationAnalyzer.DiagnosticIdMSTestTestClass)
            {
                var attribute = node.AncestorsAndSelf().OfType<AttributeSyntax>().FirstOrDefault();
                if (attribute != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Remove [TestClass]",
                            createChangedDocument: c => RemoveAttributeAsync(context.Document, attribute, c),
                            equivalenceKey: "RemoveTestClass"),
                        diagnostic);
                }
            }
            else if (diagnosticId == MigrationAnalyzer.DiagnosticIdNUnitTestCase)
            {
                var attribute = node.AncestorsAndSelf().OfType<AttributeSyntax>().FirstOrDefault();
                if (attribute != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Convert to [Theory] + [InlineData]",
                            createChangedDocument: c => MigrateTestCaseAsync(context.Document, attribute, c),
                            equivalenceKey: "MigrateTestCaseToProva"),
                        diagnostic);
                }
            }
            else if (diagnosticId == MigrationAnalyzer.DiagnosticIdXunitClassFixture)
            {
                var baseType = node.AncestorsAndSelf().OfType<BaseTypeSyntax>().FirstOrDefault();
                if (baseType != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Remove IClassFixture<T>",
                            createChangedDocument: c => RemoveInterfaceAsync(context.Document, baseType, c),
                            equivalenceKey: "RemoveIClassFixture"),
                        diagnostic);
                }
            }
        }

        private static async Task<Document> MigrateUsingToProvaAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;
            if (root == null) return document;

            // Remove the old using (Xunit or NUnit)
            var newRoot = root.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepNoTrivia);
            if (newRoot == null) return document;

            // Add "using Prova;" if not exists
            if (!newRoot.Usings.Any(u => u.Name?.ToString() == "Prova"))
            {
                var provaUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Prova"))
                    .WithTrailingTrivia(SyntaxFactory.EndOfLine("\n"));
                newRoot = newRoot.AddUsings(provaUsing);
            }

            return document.WithSyntaxRoot(newRoot!);
        }

        private static async Task<Document> ReplaceAttributeAsync(Document document, AttributeSyntax attribute, string networkName, CancellationToken cancellationToken)
        {
             var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
             if (root == null) return document;
             
             // Replace Name with newName (e.g. "Fact")
             var newAttribute = attribute.WithName(SyntaxFactory.ParseName(networkName));
             
             var newRoot = root.ReplaceNode(attribute, newAttribute);
             return document.WithSyntaxRoot(newRoot!);
        }

        private static async Task<Document> MigrateTestCaseAsync(Document document, AttributeSyntax attribute, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            var methodDeclaration = attribute.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (methodDeclaration == null) return document;
            
            // 1. Convert [TestCase(...)] to [InlineData(...)]
            var inlineDataAttribute = attribute
                .WithName(SyntaxFactory.ParseName("InlineData"));
                // Note: We keep the argument list as is, since Prova InlineData syntax matches NUnit TestCase for basic args.

            // 2. Ensure [Theory] exists
            var hasTheory = methodDeclaration.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(a => a.Name?.ToString() == "Theory");

            // We need to replace the attribute properly in the list
            var methodInOldRoot = methodDeclaration;
            var listContainingAttribute = methodInOldRoot.AttributeLists.First(al => al.Attributes.Contains(attribute));
            
            // Create new attribute list with InlineData instead of TestCase
            var newAttributes = listContainingAttribute.Attributes.Replace(attribute, inlineDataAttribute);
            var newList = listContainingAttribute.WithAttributes(newAttributes);
            
            var methodWithNewAttribute = methodInOldRoot.ReplaceNode(listContainingAttribute, newList);

            // Now handle [Theory]
            // Check if we need to add [Theory] or replace [Test]/[Fact] with [Theory]
            var testAttribute = methodWithNewAttribute.AttributeLists
                .SelectMany(al => al.Attributes)
                .FirstOrDefault(a => a.Name?.ToString() == "Test" || a.Name?.ToString() == "Fact");

            // Check if this is the first TestCase attribute to avoid duplicate Theory addition in batch fixes
            var allTestCases = methodDeclaration.AttributeLists
                .SelectMany(al => al.Attributes)
                .Where(a => a.Name?.ToString() == "TestCase" || (a.Name?.ToString().EndsWith("TestCaseAttribute", StringComparison.Ordinal) ?? false));
            
            bool isFirstTestCase = allTestCases.FirstOrDefault() == attribute;

            if (testAttribute != null)
            {
                var theoryAttribute = testAttribute.WithName(SyntaxFactory.ParseName("Theory"));
                methodWithNewAttribute = methodWithNewAttribute.ReplaceNode(testAttribute, theoryAttribute);
            }
            else if (!hasTheory && isFirstTestCase)
            {
                // Add [Theory] if it doesn't exist and we didn't just replace Test/Fact
                var theoryAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Theory"));
                var theoryList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(theoryAttribute))
                    .WithTrailingTrivia(SyntaxFactory.EndOfLine("\n"));
                
                // Insert at the beginning (index 0) so it appears before InlineData
                methodWithNewAttribute = methodWithNewAttribute.WithAttributeLists(
                    methodWithNewAttribute.AttributeLists.Insert(0, theoryList));
            }

            var finalRoot = root.ReplaceNode(methodInOldRoot, methodWithNewAttribute);
            return document.WithSyntaxRoot(finalRoot!);
        }

        private static async Task<Document> RemoveInterfaceAsync(Document document, BaseTypeSyntax baseType, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;
            
            var baseList = baseType.Parent as BaseListSyntax;
            if (baseList == null) return document;

            SyntaxNode newRoot;
            if (baseList.Types.Count == 1)
            {
                // Remove the entire base list if it only contains the interface being removed
                newRoot = root.RemoveNode(baseList, SyntaxRemoveOptions.KeepNoTrivia) ?? root;
            }
            else
            {
                // Remove only the specific interface
                newRoot = root.RemoveNode(baseType, SyntaxRemoveOptions.KeepNoTrivia) ?? root;
            }

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveAttributeAsync(Document document, AttributeSyntax attribute, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            var attributeList = attribute.Parent as AttributeListSyntax;
            if (attributeList == null) return document;

            if (attributeList.Attributes.Count == 1)
            {
                var newRoot = root.RemoveNode(attributeList, SyntaxRemoveOptions.KeepLeadingTrivia | SyntaxRemoveOptions.KeepTrailingTrivia);
                return document.WithSyntaxRoot(newRoot!);
            }
            else
            {
                var newRoot = root.RemoveNode(attribute, SyntaxRemoveOptions.KeepLeadingTrivia | SyntaxRemoveOptions.KeepTrailingTrivia);
                return document.WithSyntaxRoot(newRoot!);
            }
        }
    }
}
