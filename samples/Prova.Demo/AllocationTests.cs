using System;
using System.Threading.Tasks;
using Prova;

namespace Prova.Demo
{
    /// <summary>Tests for MaxAlloc attribute.</summary>
    [Trait("Category", "Verify")]
    public class AllocationTests
    {
        /// <summary>Safe zero allocation.</summary>
        [Fact]
        [MaxAlloc(0)]
        public void ZeroAllocation_Safe()
        {
            // Just return. Should allocate 0 bytes on heap.
            // Note: If async state machine allocates, this might fail if async. But this is void.
        }

        /// <summary>Safe small allocation.</summary>
        [Fact]
        [MaxAlloc(1024)]
        public void SmallAllocation_Safe()
        {
             var arr = new byte[512];
             Assert.NotNull(arr);
        }

        // We can't easily test "Expected Failure" in the test runner itself without assertions infrastructure 
        // that supports expected failures, or by running it and checking standard output for "Failed".
        // For now, I'll add a test that I KNOW will fail, and we will verify the console output.
        // I will comment it out by default so CI stays green, but enable it for verification run.
        
        /// <summary>Unsafe large allocation.</summary>
        [Fact(Skip = "Intentional failure for verification")]
        [MaxAlloc(10)]
        public void LargeAllocation_ShouldFail()
        {
             // Allocating 1KB > 10 bytes
             var arr = new byte[1024]; 
             Assert.NotNull(arr);
        }
    }
}
