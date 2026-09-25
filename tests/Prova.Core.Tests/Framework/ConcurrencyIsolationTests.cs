using System.Threading;
using System.Threading.Tasks;

namespace Prova.Core.Tests.Framework
{
    /// <summary>
    /// Regression tests proving that concurrency isolation attributes are honoured by the
    /// Microsoft.Testing.Platform adapter, not just by the standalone generated runner.
    /// </summary>
    /// <remarks>
    /// <para>
    /// HybridMtpAdapter originally applied only a global MaxParallel semaphore. Every other
    /// isolation attribute - [DoNotParallelize], [NotInParallel(...)] and [ParallelLimiter(...)]
    /// - was silently discarded on that code path. Because `dotnet test` and CI both run through
    /// the adapter, tests that declared they required exclusive access to a shared resource ran
    /// concurrently anyway. Nothing reported a problem; suites simply became intermittently red.
    /// </para>
    /// <para>
    /// These tests detect overlap directly rather than asserting on timing. Each one occupies a
    /// shared region for long enough that any genuinely parallel execution overlaps, and records
    /// the highest number of simultaneous occupants ever observed. If the adapter ignores the
    /// attributes that value exceeds one and these tests fail.
    /// </para>
    /// </remarks>
    public class ConcurrencyIsolationTests
    {
        private static int _inFlight;
        private static int _maxObserved;

        private static async Task OccupyAsync()
        {
            var current = Interlocked.Increment(ref _inFlight);
            InterlockedMax(ref _maxObserved, current);

            await Task.Delay(60).ConfigureAwait(false);

            Interlocked.Decrement(ref _inFlight);
        }

        private static void InterlockedMax(ref int location, int value)
        {
            int initial;
            do
            {
                initial = Volatile.Read(ref location);
                if (value <= initial)
                {
                    return;
                }
            }
            while (Interlocked.CompareExchange(ref location, value, initial) != initial);
        }

        /// <summary>Verifies tests sharing a [NotInParallel] key never overlap.</summary>
        [Fact]
        [NotInParallel("isolation-regression")]
        public async Task NotInParallel_First_Does_Not_Overlap()
        {
            await OccupyAsync();
            Assert.Equal(1, Volatile.Read(ref _maxObserved));
        }

        /// <summary>Verifies tests sharing a [NotInParallel] key never overlap.</summary>
        [Fact]
        [NotInParallel("isolation-regression")]
        public async Task NotInParallel_Second_Does_Not_Overlap()
        {
            await OccupyAsync();
            Assert.Equal(1, Volatile.Read(ref _maxObserved));
        }

        /// <summary>Verifies tests sharing a [NotInParallel] key never overlap.</summary>
        [Fact]
        [NotInParallel("isolation-regression")]
        public async Task NotInParallel_Third_Does_Not_Overlap()
        {
            await OccupyAsync();
            Assert.Equal(1, Volatile.Read(ref _maxObserved));
        }

        /// <summary>
        /// Verifies a [DoNotParallelize] test runs alone, including against the
        /// [NotInParallel] tests above, which share the same observation counter.
        /// </summary>
        [Fact]
        [DoNotParallelize]
        public async Task DoNotParallelize_Runs_Exclusively()
        {
            await OccupyAsync();
            Assert.Equal(1, Volatile.Read(ref _maxObserved));
        }
    }
}
