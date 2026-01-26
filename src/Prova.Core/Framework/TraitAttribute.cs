namespace Prova
{
    /// <summary>
    /// Attribute that is used to associate arbitrary metadata with a test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TraitAttribute : Attribute
    {
        /// <summary>Gets the name of the trait.</summary>
        public string Name { get; }
        
        /// <summary>Gets the value of the trait.</summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraitAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the trait.</param>
        /// <param name="value">The value of the trait.</param>
        public TraitAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
