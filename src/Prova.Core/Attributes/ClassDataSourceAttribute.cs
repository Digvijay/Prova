using System;

namespace Prova
{
    /// <summary>
    /// Specifies that the test data comes from a class that will be resolved via Dependency Injection.
    /// The class must implement IEnumerable&lt;object[]&gt;.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ClassDataSourceAttribute : Attribute
    {
        /// <summary>Gets the type of the class that provides the test data.</summary>
        public Type Class { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDataSourceAttribute"/> class.
        /// </summary>
        /// <param name="classType">The type of the class that provides the data.</param>
        public ClassDataSourceAttribute(Type classType)
        {
            Class = classType;
        }
    }
}
