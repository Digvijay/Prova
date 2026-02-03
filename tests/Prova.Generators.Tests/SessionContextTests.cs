using System.Threading.Tasks;
using Xunit;
using Prova.Generators.Tests;

namespace Prova.Generators.Tests.Session
{
    public class SessionContextTests
    {
        [Fact]
        public async Task Verify_Session_Context_Initialization_And_Usage()
        {
            var source = @"
using Prova;
using System.Threading.Tasks;

public class MyTests
{
    [BeforeAssembly]
    public static void Before()
    {
        var output = TestContext.Current.Output;
    }

    [Fact]
    public void Test1() { }

    [AfterAssembly]
    public static void After()
    {
        var output = TestContext.Current.Output;
    }
}
";

            GeneratorVerifier.VerifyContains(source, ExpectedSnippets);
        }

        private static readonly string[] ExpectedSnippets = new[]
        {
            // Verify Session Context Initialization
            "var sessionContext = new Prova.TestContext(\"Session\", new global::System.Collections.Generic.Dictionary<string, string>(), global::System.Threading.CancellationToken.None);",
            "Prova.TestContext.Current = sessionContext;",
            
            // Verify Session Context being saved and restored in test execution
            "var previousContext = Prova.TestContext.Current;",
            "Prova.TestContext.Current = context;",
            "Prova.TestContext.Current = previousContext;",
            
            // Verify Session Output Printing
            "var sessionOutput = sessionContext.Output.Output;",
            "global::System.Console.WriteLine(sessionOutput);"
        };
    }
}
