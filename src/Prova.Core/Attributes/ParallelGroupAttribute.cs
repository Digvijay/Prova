using System;

namespace Prova
{
    /// <summary>
    /// Specifies that tests belong to a named parallel group.
    /// Tests in different groups run in parallel.
    /// Usage dictates logical grouping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ParallelGroupAttribute : Attribute
    {
        /// <summary>Gets the name of the parallel group.</summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelGroupAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the parallel group.</param>
        public ParallelGroupAttribute(string name)
        {
            Name = name;
        }
    }
}
