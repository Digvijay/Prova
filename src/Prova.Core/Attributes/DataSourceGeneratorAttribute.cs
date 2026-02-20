using System;
using System.Collections.Generic;

namespace Prova
{
    /// <summary>
    /// Base class for custom data sources that provide data for a test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class DataSourceGeneratorAttribute : Attribute
    {
        /// <summary>
        /// Gets the data for the test method.
        /// </summary>
        /// <returns>An enumeration of data rows, where each row is an array of objects corresponding to the test parameters.</returns>
        public abstract IEnumerable<object?[]> GetData();

        /// <summary>
        /// Gets or sets the skip reason for the data source.
        /// </summary>
        public virtual string? Skip { get; set; }
    }
}
