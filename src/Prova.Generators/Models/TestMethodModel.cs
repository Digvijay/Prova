using System.Collections.Generic;

namespace Prova.Generators.Models
{
    internal sealed record HookInfo(string Name, bool IsAsync, string? ExecutorType = null);

    internal sealed record TestMethodModel(
        string ClassName, 
        string MethodName, 
        bool IsAsync, 
        bool ReturnsVoid,
        bool IsTheory, 
        List<string[]> TestData,
        List<string> Dependencies,
        List<string> FixtureTypes,
        bool UsesOutputHelper,
        bool ImplementsAsyncLifetime,
        string? Description, // <summary>
        bool IsFocused, // [Focus]
        int? RetryCount, // [Retry(n)]
        string? SkipReason,
        Dictionary<string, string> Properties,
        List<MemberDataModel> MemberData,
        List<string> ParameterTypes,
        List<string> MockFields, // New: Fields that need VerifyAll()
        bool IsStatic,
        int? MaxParallel,
        bool DoNotParallelize, // [DoNotParallelize]
        List<string> ClassData, // [ClassData(typeof(T))]
        long? MaxAllocBytes, // [MaxAlloc(bytes)]
        int? TimeoutMs, // [Timeout(ms)]
        List<HookInfo> LifecycleBefore, // [Before(Test)] methods
        List<HookInfo> LifecycleAfter, // [After(Test)] methods
        List<HookInfo> ClassBefore, // [Before(Class)] methods
        List<HookInfo> ClassAfter, // [After(Class)] methods
        List<string> ResourceConstraints, // [NotInParallel("key")]
        int? RepeatCount, // [Repeat(n)]
        string? Culture, // [Culture("name")]
        List<string[]> ClassVariants, // [Variant] on class
        List<string[]> MethodVariants, // [Variant] on method
        List<string> ClassTypeParams, // Names of class type parameters (e.g. T)
        List<string> MethodTypeParams, // Names of method type parameters
        List<string[]> ClassTestData, // [InlineData] on class
        List<MemberDataModel> ClassMemberData, // [MemberData] on class
        List<string> ClassClassData, // [ClassData] on class
        List<(string ParamName, List<string> Values)> CombinatorialValues, // [Matrix]
        string? DisplayNameFormat, // [DisplayName("Format")]
        List<string?> ParameterFormatters, // [ArgumentDisplayFormatter(typeof(T))]
        string? ParallelGroup, // [ParallelGroup("name")]
        List<(string Key, int Limit)> ParallelLimiters, // [ParallelLimiter("key", limit)]
        List<string> ClassDataSources, // [ClassDataSource(typeof(T))]
        List<MemberDataModel> MethodDataSources, // [MethodDataSource("name")]
        List<string> ClassClassDataSources, // [ClassDataSource] on class
        List<MemberDataModel> ClassMethodDataSources, // [MethodDataSource] on class
        List<CustomGeneratorModel> CustomDataGenerators,
        List<CustomGeneratorModel> ClassCustomDataGenerators,
        string? ClassFactoryType,
        List<MemberDataModel> DIDataSources,
        List<MemberDataModel> ClassDIDataSources,
        List<string> ConstructorParameterTypes, // Types of constructor parameters for injection
        string? TestExecutorType = null, // [Executor(typeof(T))]
        List<string>? NamedVariants = null, // [TestVariant("Name")]
        bool IsFsCheckProperty = false, // [Property] from FsCheck
        Dictionary<string, string>? FsCheckConfig = null
    );
}
