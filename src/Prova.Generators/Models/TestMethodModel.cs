using System.Collections.Generic;

namespace Prova.Generators.Models
{
    internal sealed record TestMethodModel(
        string ClassName, 
        string MethodName, 
        bool IsAsync, 
        bool IsTheory, 
        List<string[]> TestData,
        List<string> Dependencies,
        List<string> FixtureTypes,
        bool UsesOutputHelper,
        bool ImplementsAsyncLifetime,
        string? Description, // <summary>
        bool IsFocused, // [Focus]
        int RetryCount, // [Retry(n)]
        string? SkipReason,
        List<KeyValuePair<string, string>> Traits,
        List<MemberDataModel> MemberData,
        List<string> ParameterTypes,
        List<string> MockFields, // New: Fields that need VerifyAll()
        bool IsStatic
    );
}
