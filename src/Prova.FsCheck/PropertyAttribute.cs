using System;

namespace Prova.FsCheck
{
    /// <summary>
    /// Identifies a method as a Property-Based Test to be executed by FsCheck.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute() { }

        /// <summary>
        /// The maximum number of tests to run (MaxNbOfTest).
        /// </summary>
        public int MaxTest { get; set; } = 100;

        /// <summary>
        /// The maximum number of tests where values are rejected (MaxNbOfTest).
        /// </summary>
        public int MaxFail { get; set; } = 1000;
        
        /// <summary>
        /// The start size for the generator.
        /// </summary>
        public int StartSize { get; set; } = 1;
        
        /// <summary>
        /// The end size for the generator.
        /// </summary>
        public int EndSize { get; set; } = 100;
        
        /// <summary>
        /// Output verbose details.
        /// </summary>
        public bool Verbose { get; set; } = false;
        
        /// <summary>
        /// If true, suppress shrinking on failure.
        /// </summary>
        public bool QuietOnSuccess { get; set; } = false;
    }
}
