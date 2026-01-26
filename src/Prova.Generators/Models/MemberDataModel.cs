namespace Prova.Generators.Models
{
    internal sealed record MemberDataModel(
        string MemberName,
        string? MemberType,
        string[] Parameters,
        bool IsMethod
    );
}
