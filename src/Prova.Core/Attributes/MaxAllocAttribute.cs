using System;

namespace Prova
{
    /// <summary>
    /// Specifies the maximum number of bytes that a test is allowed to allocate on the heap.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MaxAllocAttribute : Attribute
    {
        /// <summary>Gets the maximum allowed allocation in bytes.</summary>
        public long Bytes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxAllocAttribute"/> class.
        /// </summary>
        /// <param name="bytes">The allocation limit in bytes.</param>
        public MaxAllocAttribute(long bytes)
        {
            Bytes = bytes;
        }
    }
}
