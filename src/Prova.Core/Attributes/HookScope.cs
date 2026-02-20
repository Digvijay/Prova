namespace Prova
{
    /// <summary>
    /// Specifies the scope of a lifecycle hook.
    /// </summary>
    public enum HookScope
    {
        /// <summary>The hook runs for each test.</summary>
        Test,
        /// <summary>The hook runs once for the class.</summary>
        Class,
        /// <summary>The hook runs once for the assembly.</summary>
        Assembly
    }
}
