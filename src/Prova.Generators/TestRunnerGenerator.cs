using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;
using Prova.Generators.Analysis;
using Prova.Generators.Emission;

namespace Prova.Generators
{
    [Generator]
    public class TestRunnerGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var testMethods = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => SyntaxAnalyzer.IsCandidateMethod(s),
                    transform: static (ctx, _) => SyntaxAnalyzer.GetTestMethod(ctx))
                .Where(static m => m is not null);

            var collectedTests = testMethods.Collect();

            var configureMethods = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => SyntaxAnalyzer.IsConfigureMethod(s),
                    transform: static (ctx, _) => SyntaxAnalyzer.GetConfigureMethod(ctx))
                .Where(static m => m is not null)
                .Collect();

            var assemblyHooks = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => SyntaxAnalyzer.IsAssemblyHookMethod(s),
                    transform: static (ctx, _) => SyntaxAnalyzer.GetAssemblyHook(ctx))
                .Where(static m => m is not null)
                .Collect();

            var globalHooks = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => SyntaxAnalyzer.IsGlobalHookMethod(s),
                    transform: static (ctx, _) => SyntaxAnalyzer.GetGlobalHook(ctx))
                .Where(static m => m is not null)
                .Collect();

            var testFactories = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => SyntaxAnalyzer.IsTestFactoryMethod(s),
                    transform: static (ctx, _) => SyntaxAnalyzer.GetTestFactoryMethod(ctx))
                .Where(static m => m is not null)
                .Collect();

            var globalParallel = context.CompilationProvider.Select((c, _) => {
                var attributes = c.Assembly.GetAttributes();
                var parallel = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ParallelAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ParallelAttribute");
                var sequential = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "SequentialAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.SequentialAttribute");
                
                if (sequential != null) return (int?)1;
                if (parallel != null)
                {
                    if (parallel.ConstructorArguments.Length > 0 && parallel.ConstructorArguments[0].Value is int m) return (int?)m;
                    return (int?)-1; // Parallel with no args = high parallelism
                }
                return null;
            });

            var combined = collectedTests
                .Combine(configureMethods)
                .Combine(assemblyHooks)
                .Combine(globalHooks)
                .Combine(globalParallel)
                .Combine(testFactories);

            context.RegisterSourceOutput(combined, static (spc, source) => {
                var tests = source.Left.Left.Left.Left.Left;
                var configs = source.Left.Left.Left.Left.Right;
                var hooks = source.Left.Left.Left.Right;
                var globals = source.Left.Left.Right;
                var globalMaxParallel = source.Left.Right;
                var factories = source.Right.ToList();

                var configMethods = configs.ToList();
                var beforeAssembly = hooks.Where(h => h?.HookType == "Before").Select(h => (h?.Method, h?.IsAsync ?? false, h?.ExecutorType)).ToList();
                var afterAssembly = hooks.Where(h => h?.HookType == "After").Select(h => (h?.Method, h?.IsAsync ?? false, h?.ExecutorType)).ToList();
                
                // Global hooks
                var beforeEveryTest = globals.Where(g => g?.HookType == "Before" && g?.Scope == 0).Select(g => (g?.Method, g?.IsAsync ?? false, g?.ExecutorType)).ToList();
                var afterEveryTest = globals.Where(g => g?.HookType == "After" && g?.Scope == 0).Select(g => (g?.Method, g?.IsAsync ?? false, g?.ExecutorType)).ToList();
                var beforeEveryClass = globals.Where(g => g?.HookType == "Before" && g?.Scope == 1).Select(g => (g?.Method, g?.IsAsync ?? false, g?.ExecutorType)).ToList();
                var afterEveryClass = globals.Where(g => g?.HookType == "After" && g?.Scope == 1).Select(g => (g?.Method, g?.IsAsync ?? false, g?.ExecutorType)).ToList();

                SourceEmitter.Execute(spc, tests, configMethods, beforeAssembly, afterAssembly, beforeEveryTest, afterEveryTest, beforeEveryClass, afterEveryClass, globalMaxParallel, factories);
            });

            // Automatic Entry Point: Emit Program.g.cs if no Main method is detected in user code
            // This uses the compilation to search for top-level statements or static Main methods
            var hasMainMethod = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => s is MethodDeclarationSyntax m && m.Identifier.Text == "Main" && m.Modifiers.Any(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword),
                    transform: static (ctx, _) => true)
                .Collect()
                .Select(static (items, _) => items.Length > 0);

            // Also detect top-level statements (GlobalStatementSyntax)
            var hasTopLevelStatements = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => s is GlobalStatementSyntax,
                    transform: static (ctx, _) => true)
                .Collect()
                .Select(static (items, _) => items.Length > 0);

            var hasEntryPoint = hasMainMethod.Combine(hasTopLevelStatements)
                .Select(static (pair, _) => pair.Left || pair.Right);

            context.RegisterSourceOutput(hasEntryPoint, static (spc, hasEntry) => {
                if (!hasEntry)
                {
                    var programSb = new StringBuilder();
                    programSb.AppendLine("// <auto-generated />");
                    programSb.AppendLine("// Prova Auto-Generated Entry Point");
                    programSb.AppendLine("// This file is generated by the Prova Source Generator.");
                    programSb.AppendLine("// It makes test projects self-executing console apps by default.");
                    programSb.AppendLine("// UTF-8 output is forced to ensure emoji in [DisplayName] survives all shells.");
                    programSb.AppendLine("// To provide your own entry point, add a Program.cs with a Main method or top-level statements.");
                    programSb.AppendLine();
                    programSb.AppendLine("System.Console.OutputEncoding = System.Text.Encoding.UTF8;");
                    programSb.AppendLine("System.Console.InputEncoding = System.Text.Encoding.UTF8;");
                    programSb.AppendLine("await Prova.TestRunnerExecutor.RunAllAsync(args);");

                    spc.AddSource("Program.g.cs", SourceText.From(programSb.ToString(), Encoding.UTF8));
                }
            });
        }
    }
}
