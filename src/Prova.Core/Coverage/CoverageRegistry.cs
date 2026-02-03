using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Prova
{
    /// <summary>
    /// Static registry for tracking code coverage hits at runtime.
    /// This is designed to be extremely lightweight and AOT-compatible.
    /// </summary>
    public static class CoverageRegistry
    {
        // Maps unique ID to hit count. Using a long array for maximum performance.
        // The ID is assigned at compile-time by the source generator.
        private static long[] _hits = Array.Empty<long>();
        private static string[] _metadata = Array.Empty<string>(); // "File:Line"

        /// <summary>
        /// Global lock for registry expansion.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Registers a set of probes and initializes the hit counters.
        /// Called once by the generated entry point.
        /// </summary>
        public static void Initialize(int count, string[] metadata)
        {
            lock (_lock)
            {
                _hits = new long[count];
                _metadata = metadata;
            }
        }

        /// <summary>
        /// Records a hit for a specific probe ID.
        /// </summary>
        public static void Hit(int id)
        {
            if (id >= 0 && id < _hits.Length)
            {
                global::System.Threading.Interlocked.Increment(ref _hits[id]);
            }
        }

        /// <summary>
        /// Emits the recorded coverage data in LCOV format.
        /// </summary>
        public static void EmitLcov(string filePath)
        {
            var sb = new StringBuilder();
            
            // Group hits by file
            var fileGroups = _metadata
                .Select((m, i) => new { Metadata = m, Index = i })
                .GroupBy(x => x.Metadata.Split(':')[0]);

            foreach (var group in fileGroups)
            {
                sb.AppendLine(global::System.Globalization.CultureInfo.InvariantCulture, $"SF:{group.Key}");
                
                int totalLines = 0;
                int hitLines = 0;

                foreach (var item in group)
                {
                    var parts = item.Metadata.Split(':');
                    int line;
                    if (parts.Length > 1 && int.TryParse(parts[1], out var l))
                    {
                        line = l;
                    }
                    else
                    {
                        // Fallback: Use index + 1 as line number if not provided
                        line = item.Index + 1;
                    }

                    var hits = _hits[item.Index];
                    sb.AppendLine(global::System.Globalization.CultureInfo.InvariantCulture, $"DA:{line},{hits}");
                    totalLines++;
                    if (hits > 0) hitLines++;
                }

                sb.AppendLine(global::System.Globalization.CultureInfo.InvariantCulture, $"LH:{hitLines}");
                sb.AppendLine(global::System.Globalization.CultureInfo.InvariantCulture, $"LF:{totalLines}");
                sb.AppendLine("end_of_record");
            }

            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, sb.ToString());
                global::System.Console.WriteLine($"[Coverage] Report emitted to: {filePath}");
            }
            catch (Exception ex)
            {
                global::System.Console.WriteLine($"[Coverage] Failed to emit report: {ex.Message}");
            }
        }
    }
}
