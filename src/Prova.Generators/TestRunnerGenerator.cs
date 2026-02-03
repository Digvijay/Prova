using Microsoft.CodeAnalysis;
using Prova.Generators.Analysis;
using Prova.Generators.Emission;

namespace Prova.Generators
{
    [Generator]
    public class TestRunnerGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            /*
            context.RegisterSourceOutput(context.CompilationProvider, (spc, c) => {
                 spc.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("PRVDEBUG01", "Debug", "Generator Loop Start", "Debug", DiagnosticSeverity.Warning, true), Location.None));
            });
            */
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
        }
    }
}
