using System.Collections.Generic;
using System.Linq;
using Prova.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Prova.Generators.Analysis
{
    internal static class SyntaxAnalyzer
    {
        public static bool IsCandidateMethod(SyntaxNode node)
        {
            return node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0;
        }

        public static TestMethodModel? GetTestMethod(GeneratorSyntaxContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration) as IMethodSymbol;

            if (symbol is null) return null;

            // Extract XML Documentation (Wizardry 🧙‍♂️)
            // This gets the raw XML. We want just the summary text usually.
            // Simple parsing:
            string? description = null;
            var xml = symbol.GetDocumentationCommentXml();
            if (!string.IsNullOrEmpty(xml))
            {
                // Regex or simple parsing to get <summary> content
                var match = System.Text.RegularExpressions.Regex.Match(xml, @"<summary>\s*(.*?)\s*</summary>", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (match.Success)
                {
                    description = match.Groups[1].Value.Trim().Replace("\r", "").Replace("\n", " ");
                }
            }

            var attributes = symbol.GetAttributes();
            var factAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "FactAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.FactAttribute");
            var theoryAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "TheoryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.TheoryAttribute");

            bool isFact = factAttr != null;
            bool isTheory = theoryAttr != null;

            if (!isFact && !isTheory) return null;

            string? skipReason = null;
            var activeAttr = isTheory ? theoryAttr! : factAttr!;
            var skipNamedArg = activeAttr.NamedArguments.FirstOrDefault(na => na.Key == "Skip");
            if (!skipNamedArg.Equals(default(KeyValuePair<string, TypedConstant>)))
            {
                skipReason = skipNamedArg.Value.Value as string;
            }

            // Traits (Method & Class level)
            var traits = new List<KeyValuePair<string, string>>();
            var traitAttributes = attributes.Concat(symbol.ContainingType.GetAttributes())
                                           .Where(ad => ad.AttributeClass?.Name == "TraitAttribute");

            foreach (var attr in traitAttributes)
            {
                 if (attr.ConstructorArguments.Length == 2)
                 {
                     string key = attr.ConstructorArguments[0].Value?.ToString() ?? "";
                     string val = attr.ConstructorArguments[1].Value?.ToString() ?? "";
                     traits.Add(new KeyValuePair<string, string>(key, val));
                 }
            }

            // Focus & Retry
            bool isFocused = attributes.Any(ad => ad.AttributeClass?.Name == "FocusAttribute");
            int retryCount = 0;
            var retryAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "RetryAttribute");
            if (retryAttr != null && retryAttr.ConstructorArguments.Length > 0)
            {
                if (retryAttr.ConstructorArguments[0].Value is int count)
                {
                    retryCount = count;
                }
            }

            var testData = new List<string[]>();
            if (isTheory)
            {
                var inlineDataAttrs = attributes.Where(ad => ad.AttributeClass?.Name == "InlineDataAttribute");
                foreach (var attr in inlineDataAttrs)
                {
                     if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
                     {
                         var args = attr.ConstructorArguments[0].Values.Select(v => FormatValue(v)).ToArray();
                         testData.Add(args);
                     }
                     else if (attr.ConstructorArguments.Length == 1 && attr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
                     {
                         var args = attr.ConstructorArguments[0].Values.Select(v => FormatValue(v)).ToArray();
                         testData.Add(args);
                     }
                }
            }

            var memberData = new List<MemberDataModel>();
            if (isTheory)
            {
                // MemberData
                var memberDataAttrs = attributes.Where(ad => ad.AttributeClass?.Name == "MemberDataAttribute");
                foreach (var attr in memberDataAttrs)
                {
                    if (attr.ConstructorArguments.Length > 0)
                    {
                        string memberName = attr.ConstructorArguments[0].Value?.ToString() ?? "";
                        string? memberType = null;
                        var paramsList = new List<string>();

                        // Parse extra constructor arguments (parameters)
                        if (attr.ConstructorArguments.Length > 1)
                        {
                            // If it's a params array
                             if (attr.ConstructorArguments[1].Kind == TypedConstantKind.Array)
                             {
                                 paramsList.AddRange(attr.ConstructorArguments[1].Values.Select(FormatValue));
                             }
                             else 
                             {
                                 // Individual args?
                                 for (int i = 1; i < attr.ConstructorArguments.Length; i++)
                                 {
                                     paramsList.Add(FormatValue(attr.ConstructorArguments[i]));
                                 }
                             }
                        }

                        // Check Named Arguments (MemberType)
                        // Check Named Arguments (MemberType)
                        var memberTypeArg = attr.NamedArguments.FirstOrDefault(na => na.Key == "MemberType");
                        ITypeSymbol? targetSymbol = symbol.ContainingType;

                        if (!memberTypeArg.Equals(default(KeyValuePair<string, TypedConstant>)))
                        {
                            if (memberTypeArg.Value.Value is INamedTypeSymbol typeSymbol)
                            {
                                memberType = typeSymbol.ToDisplayString();
                                targetSymbol = typeSymbol;
                            }
                        }

                        // Determine if it is a Method
                        bool isMethod = false;
                        if (targetSymbol != null)
                        {
                            var members = targetSymbol.GetMembers(memberName);
                            var member = members.FirstOrDefault();
                            if (member != null && member.Kind == SymbolKind.Method)
                            {
                                isMethod = true;
                            }
                        }

                        memberData.Add(new MemberDataModel(memberName, memberType, paramsList.ToArray(), isMethod));
                    }
                }
            }
            // NEW: Class Data
            var classDataList = new List<string>();
            foreach (var attr in attributes)
            {
                if (attr.AttributeClass?.Name == "ClassDataAttribute")
                {
                    if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is INamedTypeSymbol typeSymbol)
                    {
                        classDataList.Add(typeSymbol.ToDisplayString());
                    }
                }
            }

            // Analyze Constructor for Dependencies
            var classSymbol = symbol.ContainingType;
            var constructor = classSymbol.Constructors.FirstOrDefault(c => !c.IsStatic && c.DeclaredAccessibility == Accessibility.Public);
            var dependencies = new List<string>();
            var fixtureTypes = new List<string>();
            bool usesOutputHelper = false;

            // DI: Scan for [TestDependency] factory methods
            var compilation = context.SemanticModel.Compilation;
            var factories = new Dictionary<string, string>(); // ReturnType -> "Method()"

            // Scan all types in compilation for [TestDependency]
            // Optimization: Only scan types in the current assembly + referenced ones?
            // For now, we only scan the test project assembly to avoid perf hit.
            // Or maybe we can rely on GlobalNamespace recursive scan? No, too slow.
            // Let's scan compilation.GlobalNamespace.GetMembers() recursively?
            
            // To be efficient, we scan the symbols available in the compilation.
            // But getting ALL symbols is expensive. 
            // Alternative: User must be in same project.
            // Let's iterate namespace members of the current assembly.
            // This is a minimal implementation of "Search".
            
            var visitor = new DependencyVisitor();
            visitor.Visit(compilation.Assembly.GlobalNamespace);

            if (constructor != null)
            {
                foreach (var param in constructor.Parameters)
                {
                    var typeName = param.Type.ToDisplayString();
                    if (typeName == "Prova.ITestOutputHelper")
                    {
                        dependencies.Add("outputHelper");
                        usesOutputHelper = true;
                    }
                    else if (visitor.Factories.TryGetValue(typeName, out var factoryCall))
                    {
                        dependencies.Add(factoryCall);
                    }
                    else
                    {
                        var fixtureInterface = classSymbol.AllInterfaces.FirstOrDefault(i => 
                            i.Name == "IClassFixture" && 
                            i.TypeArguments.Length == 1 && 
                            SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], param.Type));

                        if (fixtureInterface != null)
                        {
                            var fixtureType = param.Type.ToDisplayString();
                            fixtureTypes.Add(fixtureType);
                            dependencies.Add($"fixture_{fixtureType.Replace(".", "_")}");
                        }
                        else
                        {
                            // If not found, maybe just try new T()? 
                            // Or keep default! to warn user.
                            dependencies.Add("default!"); 
                        }
                    }
                }
            }

            bool implementsAsyncLifetime = classSymbol.AllInterfaces.Any(i => i.Name == "IAsyncLifetime" || i.ToDisplayString() == "Prova.IAsyncLifetime");
            var parameterTypes = symbol.Parameters.Select(p => p.Type.ToDisplayString()).ToList();

            // Nordic Suite Integration: Smart Verify 🛡️
            // Scan fields in the containing class to see if they are Mocks
            var mockFields = new List<string>();
            var classMembers = classSymbol.GetMembers();
            foreach (var member in classMembers.OfType<IFieldSymbol>())
            {
                // Heuristic: If type starts with "Skugga." or "Mock<" or implements a known interface
                var typeName = member.Type.ToDisplayString();
                if (typeName.Contains("Skugga") || typeName.StartsWith("Mock<", global::System.StringComparison.Ordinal) || typeName.Contains(".Mock<"))
                {
                     mockFields.Add(member.Name);
                }
            }

            // Concurrency Control
            int? maxParallel = null;
            var parallelAttr = classSymbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "ParallelAttribute");
            if (parallelAttr != null && parallelAttr.ConstructorArguments.Length > 0)
            {
                if (parallelAttr.ConstructorArguments[0].Value is int max)
                {
                    maxParallel = max;
                }
            }

            return new TestMethodModel(
                symbol.ContainingType.ToDisplayString(),
                symbol.Name,
                symbol.IsAsync,
                isTheory,
                testData,
                dependencies,
                fixtureTypes.Distinct().ToList(),
                usesOutputHelper,
                implementsAsyncLifetime,
                description, // description
                isFocused, // isFocused
                retryCount, // retryCount
                skipReason, // skipReason
                traits, // traits
                memberData,
                parameterTypes,
                mockFields,
                symbol.IsStatic,
                maxParallel,
                classDataList
            );
        }

        private static string FormatValue(TypedConstant constant)
        {
            if (constant.IsNull) return "null";
            if (constant.Kind == TypedConstantKind.Type) return $"typeof({constant.Value})";
            if (constant.Kind == TypedConstantKind.Array) 
            {
                 return "new[] { " + string.Join(", ", constant.Values.Select(FormatValue)) + " }";
            }
            
            var value = constant.Value;
            if (value is string s) return $"\"{s}\"";
            if (value is bool b) return b ? "true" : "false";
            if (value is float f) return $"{f}f";
            if (value is double d) return $"{d}d";
            if (value is decimal dec) return $"{dec}m";
            return value?.ToString() ?? "null";
        }
    }

    internal sealed class DependencyVisitor : SymbolVisitor
    {
        public Dictionary<string, string> Factories { get; } = new Dictionary<string, string>();

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                member.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
             foreach (var member in symbol.GetMembers())
             {
                 member.Accept(this);
             }
        }

        public override void VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.IsStatic && symbol.DeclaredAccessibility == Accessibility.Public)
            {
                 var attr = symbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "TestDependencyAttribute");
                 if (attr != null)
                 {
                     var returnType = symbol.ReturnType.ToDisplayString();
                     var call = $"{symbol.ContainingType.ToDisplayString()}.{symbol.Name}()";
                     if (!Factories.ContainsKey(returnType))
                     {
                         Factories[returnType] = call;
                     }
                 }
            }
        }
    }
}
