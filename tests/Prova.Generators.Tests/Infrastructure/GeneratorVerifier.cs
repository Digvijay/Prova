using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Prova;
using Prova.Generators;
using Xunit;

namespace Prova.Generators.Tests
{
    public static class GeneratorVerifier
    {
        public static void Verify(string source, string expectedGeneratedSource)
        {
            // 1. Create Compilation
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ProvaTest).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location)
            };

            var compilation = CSharpCompilation.Create(
                "TestProject",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            // 2. Run Generator
            var generator = new TestRunnerGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            
            driver = driver.RunGenerators(compilation);
            var result = driver.GetRunResult();

            // 3. Verify
            // Expect 1 generated source from execution
            var runResult = result.Results[0];
            
            if (runResult.GeneratedSources.Length != 1)
            {
                // If 0, it means generator didn't run or find candidates
                Assert.Fail($"Expected 1 generated source, but found {runResult.GeneratedSources.Length}. Parsing errors: {string.Join("\n", result.Diagnostics)}");
            }

            var generatedSourceText = runResult.GeneratedSources[0].SourceText.ToString();
            
            // Normalize line endings for comparison
            var expected = expectedGeneratedSource.Replace("\r\n", "\n").Trim();
            var actual = generatedSourceText.Replace("\r\n", "\n").Trim();

            Assert.Equal(expected, actual);
        }

        public static void VerifyContains(string source, string expectedSnippet)
        {
            VerifyContains(source, new[] { expectedSnippet });
        }

        public static void VerifyContains(string source, string[] expectedSnippets)
        {
            // 1. Create Compilation
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ProvaTest).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location)
            };

            var compilation = CSharpCompilation.Create(
                "TestProject",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            // 2. Run Generator
            var generator = new TestRunnerGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            
            driver = driver.RunGenerators(compilation);
            var result = driver.GetRunResult();

            // 3. Verify
            var runResult = result.Results[0];
            
            if (runResult.GeneratedSources.Length != 1)
            {
                Assert.Fail($"Expected 1 generated source, but found {runResult.GeneratedSources.Length}. Diagnostics: {string.Join("\n", result.Diagnostics)}");
            }

            var generatedSourceText = runResult.GeneratedSources[0].SourceText.ToString();
            var actual = generatedSourceText.Replace("\r\n", "\n").Trim();

            foreach (var snippet in expectedSnippets)
            {
                var expected = snippet.Replace("\r\n", "\n").Trim();
                if (!actual.Contains(expected))
                {
                    Assert.Fail($"Generated source does not contain expected snippet.\n\nEXPECTED SNIPPET:\n{expected}\n\nACTUAL SOURCE:\n{actual}");
                }
            }
        }
    }
}
