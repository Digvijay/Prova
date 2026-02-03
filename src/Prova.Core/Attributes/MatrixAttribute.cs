using System;

namespace Prova
{
    /// <summary>
    /// Specifies a set of values for a test parameter. The test will run for every combination of values across all parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class MatrixAttribute : Attribute
    {
        /// <summary>Gets the set of values for the parameter.</summary>
        public object[] Values { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixAttribute"/> class.
        /// </summary>
        /// <param name="values">The values for the matrix.</param>
        public MatrixAttribute(params object[] values)
        {
            Values = values;
        }
    }
}
