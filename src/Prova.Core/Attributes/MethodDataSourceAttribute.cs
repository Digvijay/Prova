using System;

namespace Prova
{
    /// <summary>
    /// Specifies that the test data comes from a method. 
    /// If the method is not static, the containing class will be resolved via Dependency Injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MethodDataSourceAttribute : Attribute
    {
        /// <summary>Gets the name of the method that provides the test data.</summary>
        public string MethodName { get; }

        /// <summary>Gets the type that contains the data method. If null, the test class is used.</summary>
        public Type? MemberType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodDataSourceAttribute"/> class.
        /// </summary>
        /// <param name="methodName">The name of the method that provides the data.</param>
        public MethodDataSourceAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
