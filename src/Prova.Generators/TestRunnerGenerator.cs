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
            var testMethods = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => SyntaxAnalyzer.IsCandidateMethod(s),
                    transform: static (ctx, _) => SyntaxAnalyzer.GetTestMethod(ctx))
                .Where(static m => m is not null);

            var collectedTests = testMethods.Collect();

            context.RegisterSourceOutput(collectedTests, static (spc, methods) => SourceEmitter.Execute(spc, methods));
        }
    }
}
