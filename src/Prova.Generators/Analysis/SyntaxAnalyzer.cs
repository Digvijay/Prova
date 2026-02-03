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
            
            var classSymbol = symbol.ContainingType;
            var attributes = symbol.GetAttributes();
            var classAttributes = classSymbol.GetAttributes();
            var assemblyAttributes = context.SemanticModel.Compilation.Assembly.GetAttributes();

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

            var factAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "FactAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.FactAttribute");
            var theoryAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "TheoryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.TheoryAttribute");
            var dataSourceAttr = attributes.FirstOrDefault(ad => 
                ad.AttributeClass?.Name == "ClassDataSourceAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ClassDataSourceAttribute" || 
                ad.AttributeClass?.Name == "MethodDataSourceAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.MethodDataSourceAttribute" ||
                ad.AttributeClass?.Name == "DependencyInjectionDataSourceAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.DependencyInjectionDataSourceAttribute");

            var classDataSourceAttr = classAttributes.FirstOrDefault(ad => 
                ad.AttributeClass?.Name == "ClassDataSourceAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ClassDataSourceAttribute" || 
                ad.AttributeClass?.Name == "DependencyInjectionDataSourceAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.DependencyInjectionDataSourceAttribute");
            
            var fsCheckAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "PropertyAttribute" && (ad.AttributeClass?.ToDisplayString().StartsWith("Prova.FsCheck", global::System.StringComparison.Ordinal) == true || ad.ConstructorArguments.Length == 0));

            bool isFact = factAttr != null || fsCheckAttr != null;
            bool isTheory = theoryAttr != null || dataSourceAttr != null || classDataSourceAttr != null;

            if (!isFact && !isTheory) return null;

            string? skipReason = null;
            var activeAttr = isTheory ? (theoryAttr ?? dataSourceAttr ?? classDataSourceAttr!) : (factAttr ?? fsCheckAttr!);
            var skipNamedArg = activeAttr.NamedArguments.FirstOrDefault(na => na.Key == "Skip");
            if (!skipNamedArg.Equals(default(KeyValuePair<string, TypedConstant>)))
            {
                skipReason = skipNamedArg.Value.Value as string;
            }

            // Properties & Traits (Method & Class level)
            var properties = new Dictionary<string, string>();
            var propAttributes = classAttributes.Concat(attributes)
                                           .Where(ad => ad.AttributeClass?.Name == "PropertyAttribute" || ad.AttributeClass?.Name == "TraitAttribute"); // Trait/Property check usually relies on name, but let's be safe later? Actually Analyzer logic for Prop is simpler.

            foreach (var attr in propAttributes)
            {
                 if (attr.ConstructorArguments.Length == 2)
                 {
                     string key = attr.ConstructorArguments[0].Value?.ToString() ?? "";
                     string val = attr.ConstructorArguments[1].Value?.ToString() ?? "";
                     if (!string.IsNullOrEmpty(key))
                     {
                         properties[key] = val; // Last one wins if duplicate keys
                     }
                 }
            }


            // Exclude FsCheck PropertyAttribute from generic properties if it was picked up mistakenly (though namespace check helps)
            // But if user used simple [Property], it might clash. 
            // FsCheck Property usually has 0 args, Trait has 2.
            
            var fsCheckConfig = new Dictionary<string, string>();
            if (fsCheckAttr != null)
            {
                 foreach(var namedArg in fsCheckAttr.NamedArguments)
                 {
                     fsCheckConfig[namedArg.Key] = namedArg.Value.Value?.ToString() ?? "";
                 }
            }

            // Focus & Retry
            bool isFocused = attributes.Any(ad => ad.AttributeClass?.Name == "FocusAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.FocusAttribute");
            int? retryCount = null;
            
            // Retry Hierarchy: Method > Class > Assembly
            var retryAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "RetryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.RetryAttribute");
            if (retryAttr == null) retryAttr = classAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "RetryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.RetryAttribute");
            if (retryAttr == null) retryAttr = assemblyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "RetryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.RetryAttribute");

            if (retryAttr != null && retryAttr.ConstructorArguments.Length > 0)
            {
                if (retryAttr.ConstructorArguments[0].Value is int count)
                {
                    retryCount = count;
                }
            }

            int? repeatCount = null;
            var repeatAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "RepeatAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.RepeatAttribute");
            if (repeatAttr != null && repeatAttr.ConstructorArguments.Length > 0)
            {
                if (repeatAttr.ConstructorArguments[0].Value is int count)
                {
                    repeatCount = count;
                }
            }

            // Executor Hierarchy: Method > Class > Assembly
            string? testExecutorType = null;
            var executorAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            if (executorAttr == null) executorAttr = classAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            if (executorAttr == null) executorAttr = assemblyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");

            if (executorAttr != null && executorAttr.ConstructorArguments.Length > 0)
            {
                if (executorAttr.ConstructorArguments[0].Value is INamedTypeSymbol executorType)
                {
                    testExecutorType = executorType.ToDisplayString();
                }
            }

            // Class Factory
            string? classFactoryType = null;
            var factoryAttr = classAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ClassFactoryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ClassFactoryAttribute");
            if (factoryAttr != null && factoryAttr.ConstructorArguments.Length > 0)
            {
                if (factoryAttr.ConstructorArguments[0].Value is INamedTypeSymbol factoryType)
                {
                    classFactoryType = factoryType.ToDisplayString();
                }
            }

            var testData = GetInlineData(attributes);
            var memberData = GetMemberData(symbol, attributes);
            var classDataList = CollectClassData(attributes);

            var classTestData = GetInlineData(classAttributes);
            var classMemberData = GetMemberData(null, classAttributes, classSymbol);
            var classClassData = CollectClassData(classAttributes);

            var classDataSources = GetClassDataSources(attributes);
            var methodDataSources = GetMethodDataSources(symbol, attributes);
            var classClassDataSources = GetClassDataSources(classAttributes);
            var classMethodDataSources = GetMethodDataSources(null, classAttributes, classSymbol);
            var customDataGenerators = GetCustomDataGenerators(attributes);
            var classCustomDataGenerators = GetCustomDataGenerators(classAttributes);

            var diDataSources = GetDIDataSources(attributes);
            var classDIDataSources = GetDIDataSources(classAttributes);

            // Analyze Constructor for Dependencies
            var constructor = classSymbol.Constructors.FirstOrDefault(c => !c.IsStatic && c.DeclaredAccessibility == Accessibility.Public);
            var dependencies = new List<string>();
            var fixtureTypes = new List<string>();
            var constructorParameterTypes = new List<string>();
            bool usesOutputHelper = false;

            // DI: Scan for [TestDependency] factory methods
            var compilation = context.SemanticModel.Compilation;
            var visitor = new DependencyVisitor();
            visitor.Visit(compilation.Assembly.GlobalNamespace);

            if (constructor != null)
            {
                // Determine which parameters are managed by class-level data
                int dataParamCount = classTestData.Count > 0 ? classTestData[0].Length : 
                                    (classMemberData.Count > 0 ? 0 : 0); // Need more logic for memberdata/classdata if they provide fixed widths
                
                // For now, we assume all constructor parameters AFTER ITestOutputHelper or Fixtures might be data-driven
                // Actually, the SourceEmitter will handle the actual data passing.
                // We just need to know if the dependency is a "Data" parameter.
                
                int paramIndex = 0;
                foreach (var param in constructor.Parameters)
                {
                    var typeName = param.Type.ToDisplayString();
                    constructorParameterTypes.Add(typeName);
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
                            // Optimized heuristic for dependencies:
                            // If it's a known factory/fixture, it's already handled.
                            // If there is class-level data, we assume it's data-driven.
                            // Otherwise, it MUST be resolved from DI.
                            bool hasClassLevelData = classTestData.Count > 0 || classMemberData.Count > 0 || classClassData.Count > 0 || classClassDataSources.Count > 0 || classMethodDataSources.Count > 0 || classCustomDataGenerators.Count > 0;
                            if (hasClassLevelData)
                            {
                                dependencies.Add($"__CLASS_DATA_{paramIndex}__");
                            }
                            else
                            {
                                dependencies.Add($"TestRunnerExecutor.Services.Get<{typeName}>()");
                            }
                        }
                    }
                    paramIndex++;
                }
            }
            
            bool implementsAsyncLifetime = classSymbol.AllInterfaces.Any(i => i.Name == "IAsyncLifetime" || i.ToDisplayString() == "Prova.IAsyncLifetime");
            var parameterTypes = symbol.Parameters.Select(p => p.Type.ToDisplayString()).ToList();

            // Nordic Suite Integration: Smart Verify 🛡️
            var mockFields = new List<string>();
            var classMembers = classSymbol.GetMembers();
            foreach (var member in classMembers.OfType<IFieldSymbol>())
            {
                var typeName = member.Type.ToDisplayString();
                if (typeName.Contains("Skugga") || typeName.StartsWith("Mock<", global::System.StringComparison.Ordinal) || typeName.Contains(".Mock<"))
                {
                     mockFields.Add(member.Name);
                }
            }

            // Concurrency Control
            int? maxParallel = null;
            bool doNotParallelize = false;
            var concurrencyAttributes = classAttributes.Concat(attributes);

            // Parallelism Hierarchy: Method > Class > Assembly
            // 1. Check for DoNotParallelize (Method > Class)
            var doNotParallelMethod = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "DoNotParallelizeAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.DoNotParallelizeAttribute");
            var doNotParallelClass = classAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "DoNotParallelizeAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.DoNotParallelizeAttribute");
            
            if (doNotParallelMethod != null || doNotParallelClass != null)
            {
                doNotParallelize = true;
            }
            else
            {
                // 2. Check for Sequential/Serial (Method > Class > Assembly)
                var sequentialMethod = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "SequentialAttribute" || ad.AttributeClass?.Name == "SerialAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.SequentialAttribute");
                var sequentialClass = classAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "SequentialAttribute" || ad.AttributeClass?.Name == "SerialAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.SequentialAttribute");
                var sequentialAssembly = assemblyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "SequentialAttribute" || ad.AttributeClass?.Name == "SerialAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.SequentialAttribute");

                if (sequentialMethod != null) maxParallel = 1;
                else if (sequentialClass != null) maxParallel = 1;
                else if (sequentialAssembly != null && attributes.All(a => a.AttributeClass?.Name != "ParallelAttribute") && classAttributes.All(a => a.AttributeClass?.Name != "ParallelAttribute")) maxParallel = 1;
                
                // 3. Check for Parallel (Method > Class > Assembly)
                if (maxParallel == null)
                {
                    var parallelMethod = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ParallelAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ParallelAttribute");
                    var parallelClass = classAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ParallelAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ParallelAttribute");
                    var parallelAssembly = assemblyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ParallelAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ParallelAttribute");

                    if (parallelMethod != null) 
                    {
                         if (parallelMethod.ConstructorArguments.Length > 0 && parallelMethod.ConstructorArguments[0].Value is int max) maxParallel = max;
                    }
                    else if (parallelClass != null)
                    {
                         if (parallelClass.ConstructorArguments.Length > 0 && parallelClass.ConstructorArguments[0].Value is int max) maxParallel = max;
                    }
                    else if (parallelAssembly != null)
                    {
                         if (parallelAssembly.ConstructorArguments.Length > 0 && parallelAssembly.ConstructorArguments[0].Value is int max) maxParallel = max;
                    }
                }
            }

            // MaxAlloc (Method Only for now)
            long? maxAllocBytes = null;
            var maxAllocAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "MaxAllocAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.MaxAllocAttribute");
            if (maxAllocAttr != null && maxAllocAttr.ConstructorArguments.Length > 0)
            {
                if (maxAllocAttr.ConstructorArguments[0].Value is long bytes) maxAllocBytes = bytes;
            }

            // Timeout Hierarchy: Method > Class > Assembly
            int? timeoutMs = null;
            var timeoutAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "TimeoutAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.TimeoutAttribute");
            if (timeoutAttr == null) timeoutAttr = classAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "TimeoutAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.TimeoutAttribute");
            if (timeoutAttr == null) timeoutAttr = assemblyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "TimeoutAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.TimeoutAttribute");

            if (timeoutAttr != null && timeoutAttr.ConstructorArguments.Length > 0)
            {
                if (timeoutAttr.ConstructorArguments[0].Value is int ms) timeoutMs = ms;
            }

            var resourceConstraints = new List<string>();
            var notInParallelAttrs = concurrencyAttributes.Where(ad => ad.AttributeClass?.Name == "NotInParallelAttribute" || ad.AttributeClass?.Name == "NotInParallel" || ad.AttributeClass?.ToDisplayString().EndsWith(".NotInParallelAttribute", global::System.StringComparison.Ordinal) == true);
            foreach (var attr in notInParallelAttrs)
            {
                foreach (var arg in attr.ConstructorArguments)
                {
                    if (arg.Kind == TypedConstantKind.Array) resourceConstraints.AddRange(arg.Values.Select(v => v.Value?.ToString() ?? ""));
                    else { var val = arg.Value?.ToString(); if (val != null) resourceConstraints.Add(val); }
                }
            }



            string? culture = null;
            var cultureAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "CultureAttribute" || ad.AttributeClass?.Name == "Culture" || ad.AttributeClass?.ToDisplayString() == "Prova.CultureAttribute");
            if (cultureAttr != null && cultureAttr.ConstructorArguments.Length > 0) culture = cultureAttr.ConstructorArguments[0].Value?.ToString();

            // Parallel Groups and Limiters
            string? parallelGroup = null;
            var groupAttr = concurrencyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ParallelGroupAttribute" || ad.AttributeClass?.Name == "ParallelGroup");
            if (groupAttr != null && groupAttr.ConstructorArguments.Length > 0)
                parallelGroup = groupAttr.ConstructorArguments[0].Value?.ToString();

            var parallelLimiters = new List<(string Key, int Limit)>();
            var limiterAttrs = concurrencyAttributes.Where(ad => ad.AttributeClass?.Name == "ParallelLimiterAttribute" || ad.AttributeClass?.Name == "ParallelLimiter");
            foreach (var attr in limiterAttrs)
            {
                if (attr.ConstructorArguments.Length == 2)
                {
                    string key = attr.ConstructorArguments[0].Value?.ToString() ?? "";
                    if (attr.ConstructorArguments[1].Value is int limit)
                        parallelLimiters.Add((key, limit));
                }
            }

            var lifecycleBefore = new List<HookInfo>();
            var lifecycleAfter = new List<HookInfo>();
            var classBefore = new List<HookInfo>();
            var classAfter = new List<HookInfo>();

            // Hook Traversal (Inheritance Support)
            var currentType = classSymbol;
            var seenBefore = new HashSet<string>();
            var seenAfter = new HashSet<string>();
            var seenClassBefore = new HashSet<string>();
            var seenClassAfter = new HashSet<string>();

            while (currentType != null && currentType.SpecialType != SpecialType.System_Object)
            {
                foreach (var member in currentType.GetMembers().OfType<IMethodSymbol>())
                {
                    // Basic checks
                    if (member.DeclaredAccessibility != Accessibility.Public) continue;

                    var memberAttributes = member.GetAttributes();
                    
                    // -- Before Hooks --
                    var beforeAttr = memberAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "BeforeAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.BeforeAttribute");
                    var beforeClassAttr = memberAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "BeforeClassAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.BeforeClassAttribute");
                    var beforeAssemblyAttr = memberAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "BeforeAssemblyAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.BeforeAssemblyAttribute"); // Scope 2 ignored here if handled elsewhere? Actually syntax analyzer handles models.

                    if ((beforeAttr != null || beforeClassAttr != null || beforeAssemblyAttr != null) && !seenBefore.Contains(member.Name) && !seenClassBefore.Contains(member.Name))
                    {
                        int scope = 0;
                        if (beforeClassAttr != null) scope = 1;
                        else if (beforeAssemblyAttr != null) scope = 2;
                        else
                        {
                            var scopeArg = beforeAttr!.ConstructorArguments.FirstOrDefault();
                            scope = scopeArg.Value is int s ? s : 0;
                        }

                        bool isAsync = member.IsAsync || member.ReturnType.Name == "Task" || member.ReturnType.Name == "ValueTask";
                        string? hookExecutorType = GetExecutorType(memberAttributes, classAttributes, assemblyAttributes);

                        if (scope == 0 && !member.IsStatic)
                        {
                             if (seenBefore.Add(member.Name))
                                 lifecycleBefore.Add(new HookInfo(member.Name, isAsync, hookExecutorType));
                        }
                        else if (scope == 1 && member.IsStatic)
                        {
                             if (seenClassBefore.Add(member.Name))
                                 classBefore.Add(new HookInfo(member.Name, isAsync, hookExecutorType));
                        }
                    }

                    // -- After Hooks --
                    var afterAttr = memberAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "AfterAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.AfterAttribute");
                    var afterClassAttr = memberAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "AfterClassAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.AfterClassAttribute");
                    var afterAssemblyAttr = memberAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "AfterAssemblyAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.AfterAssemblyAttribute");

                    if ((afterAttr != null || afterClassAttr != null || afterAssemblyAttr != null) && !seenAfter.Contains(member.Name) && !seenClassAfter.Contains(member.Name))
                    {
                        int scope = 0;
                        if (afterClassAttr != null) scope = 1;
                        else if (afterAssemblyAttr != null) scope = 2;
                        else
                        {
                            var scopeArg = afterAttr!.ConstructorArguments.FirstOrDefault();
                            scope = scopeArg.Value is int s ? s : 0;
                        }

                        bool isAsync = member.IsAsync || member.ReturnType.Name == "Task" || member.ReturnType.Name == "ValueTask";
                        string? hookExecutorType = GetExecutorType(memberAttributes, classAttributes, assemblyAttributes);

                        if (scope == 0 && !member.IsStatic)
                        {
                             if (seenAfter.Add(member.Name))
                                 lifecycleAfter.Add(new HookInfo(member.Name, isAsync, hookExecutorType));
                        }
                        else if (scope == 1 && member.IsStatic)
                        {
                             if (seenClassAfter.Add(member.Name))
                                 classAfter.Add(new HookInfo(member.Name, isAsync, hookExecutorType));
                        }
                    }
                }
                currentType = currentType.BaseType;
            }

            // Reorder for correct execution flow
            // Before: Base -> Derived (Reverse the order we collected: Derived -> Base)
            lifecycleBefore.Reverse();
            classBefore.Reverse();
            
            // After: Derived -> Base (Already in collected order)


            var methodVariants = CollectVariants(attributes);
            var classVariants = CollectVariants(classAttributes);
            var namedVariants = new List<string>();
            try {
                namedVariants = CollectNamedVariants(attributes);
            } catch { }

            var classTypeParams = classSymbol.TypeParameters.Select(tp => tp.Name).ToList();
            var methodTypeParams = symbol.TypeParameters.Select(tp => tp.Name).ToList();

            return new TestMethodModel(
                classSymbol.ToDisplayString(),
                symbol.Name,
                symbol.IsAsync,
                symbol.ReturnsVoid,
                isTheory,
                testData,
                dependencies,
                fixtureTypes.Distinct().ToList(),
                usesOutputHelper,
                implementsAsyncLifetime,
                description,
                isFocused,
                retryCount,
                skipReason,
                properties,
                memberData,
                parameterTypes,
                mockFields,
                symbol.IsStatic,
                maxParallel,
                doNotParallelize,
                classDataList,
                maxAllocBytes,
                timeoutMs,
                lifecycleBefore,
                lifecycleAfter,
                classBefore,
                classAfter,
                resourceConstraints,
                repeatCount,
                culture,
                classVariants,
                methodVariants,
                classTypeParams,
                methodTypeParams,
                classTestData,
                classMemberData,
                classClassData,
                CollectCombinatorialValues(symbol),
                symbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "DisplayNameAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.DisplayNameAttribute")?.ConstructorArguments.FirstOrDefault().Value?.ToString(),
                CollectParameterFormatters(symbol),
                parallelGroup,
                parallelLimiters,
                classDataSources,
                methodDataSources,
                classClassDataSources,
                classMethodDataSources,
                customDataGenerators,
                classCustomDataGenerators,
                classFactoryType,
                diDataSources,
                classDIDataSources,
                constructorParameterTypes,
                testExecutorType,
                namedVariants,
                fsCheckAttr != null,
                fsCheckConfig
            );
        }

        private static List<CustomGeneratorModel> GetCustomDataGenerators(IEnumerable<AttributeData> attributes)
        {
            var results = new List<CustomGeneratorModel>();
            foreach (var attr in attributes)
            {
                if (attr.AttributeClass != null && IsDerivedFrom(attr.AttributeClass, "Prova.DataSourceGeneratorAttribute"))
                {
                    var args = attr.ConstructorArguments.Select(arg => FormatValue(arg)).ToArray();
                    results.Add(new CustomGeneratorModel(attr.AttributeClass.ToDisplayString(), args));
                }
            }
            return results;
        }

        private static bool IsDerivedFrom(ITypeSymbol symbol, string baseTypeName)
        {
            var current = symbol.BaseType;
            while (current != null)
            {
                if (current.ToDisplayString() == baseTypeName) return true;
                current = current.BaseType;
            }
            return false;
        }

        private static List<string> GetClassDataSources(IEnumerable<AttributeData> attributes)
        {
            var results = new List<string>();
            var attrs = attributes.Where(ad => ad.AttributeClass?.Name == "ClassDataSourceAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ClassDataSourceAttribute");
            foreach (var attr in attrs)
            {
                if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is INamedTypeSymbol typeSymbol)
                {
                    results.Add(typeSymbol.ToDisplayString());
                }
            }
            return results;
        }

        private static List<MemberDataModel> GetMethodDataSources(IMethodSymbol? symbol, IEnumerable<AttributeData> attributes, INamedTypeSymbol? fallbackSymbol = null)
        {
            var results = new List<MemberDataModel>();
            var attrs = attributes.Where(ad => ad.AttributeClass?.Name == "MethodDataSourceAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.MethodDataSourceAttribute");
            var containingType = symbol?.ContainingType ?? fallbackSymbol;

            foreach (var attr in attrs)
            {
                if (attr.ConstructorArguments.Length > 0)
                {
                    string methodName = attr.ConstructorArguments[0].Value?.ToString() ?? "";
                    string? memberType = null;
                    
                    var memberTypeArg = attr.NamedArguments.FirstOrDefault(na => na.Key == "MemberType");
                    ITypeSymbol? targetSymbol = containingType;

                    if (!memberTypeArg.Equals(default(KeyValuePair<string, TypedConstant>)))
                    {
                        if (memberTypeArg.Value.Value is INamedTypeSymbol typeSymbol)
                        {
                            memberType = typeSymbol.ToDisplayString();
                            targetSymbol = typeSymbol;
                        }
                    }

                    bool isMethod = false;
                    var paramsList = new List<string>();
                    if (targetSymbol != null)
                    {
                        var member = targetSymbol.GetMembers(methodName).FirstOrDefault();
                        if (member?.Kind == SymbolKind.Method && member is IMethodSymbol ms)
                        {
                            isMethod = true;
                            // Check for parameters that need DI
                            foreach (var p in ms.Parameters)
                            {
                                paramsList.Add($"TestRunnerExecutor.Services.Get<{p.Type.ToDisplayString()}>()");
                            }
                        }
                    }

                    results.Add(new MemberDataModel(methodName, memberType, paramsList.ToArray(), isMethod));
                }
            }
            return results;
        }

        private static List<string[]> GetInlineData(IEnumerable<AttributeData> attributes)
        {
            var results = new List<string[]>();
            var inlineDataAttrs = attributes.Where(ad => ad.AttributeClass?.Name == "InlineDataAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.InlineDataAttribute");
            foreach (var attr in inlineDataAttrs)
            {
                if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
                {
                    results.Add(attr.ConstructorArguments[0].Values.Select(FormatValue).ToArray());
                }
            }
            return results;
        }

        private static List<MemberDataModel> GetMemberData(IMethodSymbol? symbol, IEnumerable<AttributeData> attributes, INamedTypeSymbol? fallbackSymbol = null)
        {
            var results = new List<MemberDataModel>();
            var memberDataAttrs = attributes.Where(ad => ad.AttributeClass?.Name == "MemberDataAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.MemberDataAttribute");
            var containingType = symbol?.ContainingType ?? fallbackSymbol;

            foreach (var attr in memberDataAttrs)
            {
                if (attr.ConstructorArguments.Length > 0)
                {
                    string memberName = attr.ConstructorArguments[0].Value?.ToString() ?? "";
                    string? memberType = null;
                    var paramsList = new List<string>();

                    if (attr.ConstructorArguments.Length > 1)
                    {
                        if (attr.ConstructorArguments[1].Kind == TypedConstantKind.Array)
                            paramsList.AddRange(attr.ConstructorArguments[1].Values.Select(FormatValue));
                        else
                            for (int i = 1; i < attr.ConstructorArguments.Length; i++)
                                paramsList.Add(FormatValue(attr.ConstructorArguments[i]));
                    }

                    var memberTypeArg = attr.NamedArguments.FirstOrDefault(na => na.Key == "MemberType");
                    ITypeSymbol? targetSymbol = containingType;

                    if (!memberTypeArg.Equals(default(KeyValuePair<string, TypedConstant>)))
                    {
                        if (memberTypeArg.Value.Value is INamedTypeSymbol typeSymbol)
                        {
                            memberType = typeSymbol.ToDisplayString();
                            targetSymbol = typeSymbol;
                        }
                    }

                    bool isMethod = false;
                    if (targetSymbol != null)
                    {
                        var member = targetSymbol.GetMembers(memberName).FirstOrDefault();
                        if (member is IMethodSymbol dataMethod)
                        {
                            isMethod = true;
                            if (paramsList.Count == 0)
                            {
                                foreach (var p in dataMethod.Parameters)
                                {
                                    paramsList.Add($"TestRunnerExecutor.Services.Get<{p.Type.ToDisplayString()}>()");
                                }
                            }
                        }
                    }

                    results.Add(new MemberDataModel(memberName, memberType, paramsList.ToArray(), isMethod));
                }
            }
            return results;
        }

        private static List<MemberDataModel> GetDIDataSources(IEnumerable<AttributeData> attributes)
        {
            var results = new List<MemberDataModel>();
            var diDataSourceAttrs = attributes.Where(ad => ad.AttributeClass?.Name == "DependencyInjectionDataSourceAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.DependencyInjectionDataSourceAttribute");

            foreach (var attr in diDataSourceAttrs)
            {
                if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is INamedTypeSymbol typeSymbol)
                {
                    string providerType = typeSymbol.ToDisplayString();
                    string? memberName = null;
                    if (attr.ConstructorArguments.Length > 1)
                    {
                        memberName = attr.ConstructorArguments[1].Value?.ToString();
                    }

                    bool isMethod = false;
                    var paramsList = new List<string>();
                    if (!string.IsNullOrEmpty(memberName))
                    {
                        var member = typeSymbol.GetMembers(memberName!).FirstOrDefault();
                        if (member is IMethodSymbol dataMethod)
                        {
                            isMethod = true;
                            foreach (var p in dataMethod.Parameters)
                            {
                                paramsList.Add($"TestRunnerExecutor.Services.Get<{p.Type.ToDisplayString()}>()");
                            }
                        }
                    }

                    results.Add(new MemberDataModel(memberName ?? "", providerType, paramsList.ToArray(), isMethod));
                }
            }
            return results;
        }

        private static List<string> CollectClassData(IEnumerable<AttributeData> attributes)
        {
            var results = new List<string>();
            var classDataAttrs = attributes.Where(ad => ad.AttributeClass?.Name == "ClassDataAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ClassDataAttribute");
            foreach (var attr in classDataAttrs)
            {
                if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is INamedTypeSymbol typeSymbol)
                {
                    results.Add(typeSymbol.ToDisplayString());
                }
            }
            return results;
        }

        private static List<string> CollectNamedVariants(IEnumerable<AttributeData> attributes)
        {
            var results = new List<string>();
            var variantAttrs = attributes.Where(ad => ad.AttributeClass?.Name == "TestVariantAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.TestVariantAttribute");
            foreach (var attr in variantAttrs)
            {
                if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is string name)
                {
                    results.Add(name);
                }
            }
            return results;
        }

        private static List<string[]> CollectVariants(IEnumerable<AttributeData> attributes)
        {
            var results = new List<string[]>();
            var variantAttrs = attributes.Where(ad => ad.AttributeClass?.Name == "VariantAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.VariantAttribute");
            foreach (var attr in variantAttrs)
            {
                if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
                {
                    results.Add(attr.ConstructorArguments[0].Values.Select(v => v.Value is ITypeSymbol ts ? ts.ToDisplayString() : v.Value?.ToString() ?? "object").ToArray());
                }
            }
            return results;
        }

        private static List<(string ParamName, List<string> Values)> CollectCombinatorialValues(IMethodSymbol methodSymbol)
        {
            var results = new List<(string ParamName, List<string> Values)>();
            foreach (var param in methodSymbol.Parameters)
            {
                var matrixAttr = param.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "MatrixAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.MatrixAttribute");
                if (matrixAttr != null && matrixAttr.ConstructorArguments.Length > 0 && matrixAttr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
                {
                    var values = matrixAttr.ConstructorArguments[0].Values.Select(FormatValue).ToList();
                    results.Add((param.Name, values));
                }
            }
            return results;
        }

        private static List<string?> CollectParameterFormatters(IMethodSymbol method)
        {
            var results = new List<string?>();
            foreach (var param in method.Parameters)
            {
                var attr = param.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "ArgumentDisplayFormatterAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ArgumentDisplayFormatterAttribute");
                string? formatterType = null;
                
                if (attr != null && attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is INamedTypeSymbol typeSymbol)
                {
                    formatterType = typeSymbol.ToDisplayString();
                }
                
                results.Add(formatterType);
            }
            return results;
        }

        public static bool IsConfigureMethod(SyntaxNode node)
        {
            return node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0;
        }

        public static string? GetConfigureMethod(GeneratorSyntaxContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
            if (symbol is null || !symbol.IsStatic) return null;

            var attr = symbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "ConfigureServicesAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ConfigureServicesAttribute");
            if (attr != null)
            {
                 return $"{symbol.ContainingType.ToDisplayString()}.{symbol.Name}";
            }
            return null;
        }

        private static string? GetExecutorType(IEnumerable<AttributeData> memberAttributes, IEnumerable<AttributeData> classAttributes, IEnumerable<AttributeData> assemblyAttributes)
        {
            string? hookExecutorType = null;
            var hookExecAttr = memberAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            if (hookExecAttr == null) hookExecAttr = classAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            if (hookExecAttr == null) hookExecAttr = assemblyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            
            if (hookExecAttr != null && hookExecAttr.ConstructorArguments.Length > 0)
            {
                if (hookExecAttr.ConstructorArguments[0].Value is INamedTypeSymbol executorType)
                {
                    hookExecutorType = executorType.ToDisplayString();
                }
            }
            return hookExecutorType;
        }

        public static bool IsAssemblyHookMethod(SyntaxNode node)
        {
            return node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0;
        }

        public static (string Method, string HookType, bool IsAsync, string? ExecutorType)? GetAssemblyHook(GeneratorSyntaxContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
            if (symbol is null || !symbol.IsStatic) return null;

            var attributes = symbol.GetAttributes();
            var assemblyAttributes = context.SemanticModel.Compilation.Assembly.GetAttributes();

            var beforeAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "BeforeAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.BeforeAttribute");
            var beforeAssemblyAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "BeforeAssemblyAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.BeforeAssemblyAttribute");
            
            var afterAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "AfterAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.AfterAttribute");
            var afterAssemblyAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "AfterAssemblyAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.AfterAssemblyAttribute");

            string? executorType = null;
            var execAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            if (execAttr == null) execAttr = assemblyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            
            if (execAttr != null && execAttr.ConstructorArguments.Length > 0 && execAttr.ConstructorArguments[0].Value is INamedTypeSymbol et)
            {
                executorType = et.ToDisplayString();
            }

            bool isAsync = symbol.IsAsync || symbol.ReturnType.Name == "Task" || symbol.ReturnType.Name == "ValueTask" || symbol.ReturnType.ToDisplayString().EndsWith(".Task", global::System.StringComparison.Ordinal);

            if (beforeAttr != null || beforeAssemblyAttr != null)
            {
                int scope = 0;
                if (beforeAssemblyAttr != null) scope = 2;
                else
                {
                    var scopeArg = beforeAttr!.ConstructorArguments.FirstOrDefault();
                    scope = scopeArg.Value is int s ? s : 0;
                }

                if (scope == 2) // HookScope.Assembly
                {
                    return ($"{symbol.ContainingType.ToDisplayString()}.{symbol.Name}", "Before", isAsync, executorType);
                }
            }

            if (afterAttr != null || afterAssemblyAttr != null)
            {
                int scope = 0;
                if (afterAssemblyAttr != null) scope = 2;
                else
                {
                    var scopeArg = afterAttr!.ConstructorArguments.FirstOrDefault();
                    scope = scopeArg.Value is int s ? s : 0;
                }

                if (scope == 2) // HookScope.Assembly
                {
                    return ($"{symbol.ContainingType.ToDisplayString()}.{symbol.Name}", "After", isAsync, executorType);
                }
            }

            return null;
        }

        public static bool IsGlobalHookMethod(SyntaxNode node)
        {
            return node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0;
        }

        public static (string Method, string HookType, int Scope, bool IsAsync, string? ExecutorType)? GetGlobalHook(GeneratorSyntaxContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
            if (symbol is null || !symbol.IsStatic) return null;

            var attributes = symbol.GetAttributes();
            var assemblyAttributes = context.SemanticModel.Compilation.Assembly.GetAttributes();

            string? executorType = null;
            var execAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            if (execAttr == null) execAttr = assemblyAttributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "ExecutorAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.ExecutorAttribute");
            
            if (execAttr != null && execAttr.ConstructorArguments.Length > 0 && execAttr.ConstructorArguments[0].Value is INamedTypeSymbol et)
            {
                executorType = et.ToDisplayString();
            }

            var beforeEveryAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "BeforeEveryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.BeforeEveryAttribute");
            var afterEveryAttr = attributes.FirstOrDefault(ad => ad.AttributeClass?.Name == "AfterEveryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.AfterEveryAttribute");

            bool isAsync = symbol.IsAsync || symbol.ReturnType.ToDisplayString().EndsWith(".Task", global::System.StringComparison.Ordinal) || symbol.ReturnType.ToDisplayString().EndsWith(".ValueTask", global::System.StringComparison.Ordinal);

            if (beforeEveryAttr != null)
            {
                var scopeArg = beforeEveryAttr.ConstructorArguments.FirstOrDefault();
                int scope = scopeArg.Value is int val ? val : 0; // Default: HookScope.Test
                return ($"{symbol.ContainingType.ToDisplayString()}.{symbol.Name}", "Before", scope, isAsync, executorType);
            }

            if (afterEveryAttr != null)
            {
                var scopeArg = afterEveryAttr.ConstructorArguments.FirstOrDefault();
                int scope = scopeArg.Value is int val ? val : 0; // Default: HookScope.Test
                return ($"{symbol.ContainingType.ToDisplayString()}.{symbol.Name}", "After", scope, isAsync, executorType);
            }

            return null;
        }

        public static bool IsTestFactoryMethod(SyntaxNode node)
        {
            return node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0;
        }

        public static string? GetTestFactoryMethod(GeneratorSyntaxContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
            if (symbol is null || !symbol.IsStatic) return null;

            var attr = symbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "TestFactoryAttribute" || ad.AttributeClass?.ToDisplayString() == "Prova.TestFactoryAttribute");
            if (attr != null)
            {
                 return $"{symbol.ContainingType.ToDisplayString()}.{symbol.Name}";
            }
            return null;
        }

        private static string FormatValue(TypedConstant constant)
        {
            if (constant.IsNull) return "null";
            if (constant.Kind == TypedConstantKind.Type) return $"typeof({constant.Value})";
            if (constant.Kind == TypedConstantKind.Array) 
            {
                 return "new[] { " + string.Join(", ", constant.Values.Select(FormatValue)) + " }";
            }
            if (constant.Kind == TypedConstantKind.Enum)
            {
                return $"({constant.Type!.ToDisplayString()}){constant.Value}";
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
