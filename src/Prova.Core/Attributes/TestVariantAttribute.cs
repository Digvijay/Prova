using System;

namespace Prova
{
    /// <summary>
    /// Marking a test method or class with this attribute will cause it to be executed
    /// multiple times, once for each defined variant.
    /// The variant name is accessible via <see cref="TestContext.Variant"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TestVariantAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the variant.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestVariantAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the variant.</param>
        public TestVariantAttribute(string name)
        {
            Name = name;
        }
    }
}
