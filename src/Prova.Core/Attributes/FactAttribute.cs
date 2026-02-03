namespace Prova
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FactAttribute : Attribute
    {
        /// <summary>Gets or sets the skip reason.</summary>
        public string? Skip { get; set; }
    }
}
