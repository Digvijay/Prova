namespace Prova.Reporters
{
    /// <summary>
    /// A test reporter that outputs to the console.
    /// </summary>
    public class ConsoleReporter : ITestReporter
    {
        private readonly object _lock = new object();

        /// <inheritdoc />
        public void OnTestStarting(string testName, string? description = null)
        {
            if (!string.IsNullOrEmpty(description))
            {
                 lock (_lock)
                 {
                     Console.ForegroundColor = ConsoleColor.Cyan;
                     Console.WriteLine($"Starting: {testName}");
                     Console.ForegroundColor = ConsoleColor.DarkGray;
                     Console.WriteLine($"  \"{description}\"");
                     Console.ResetColor();
                 }
            }
        }

        /// <inheritdoc />
        public void OnTestSuccess(string testName, string output)
        {
            lock (_lock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("✓ ");
                Console.ResetColor();
                Console.WriteLine(testName);
                if (!string.IsNullOrEmpty(output))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(output);
                    Console.ResetColor();
                }
            }
        }

        /// <inheritdoc />
        public void OnTestFailure(string testName, Exception ex, string output)
        {
             lock (_lock)
             {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("✗ ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{testName}: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(ex.StackTrace);
                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine("Output:");
                    Console.WriteLine(output);
                }
                Console.ResetColor();
             }
        }

        /// <inheritdoc />
        public void OnTestSkipped(string testName, string reason)
        {
             lock (_lock)
             {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("⚠ ");
                Console.ResetColor();
                Console.WriteLine($"{testName} (Skipped: {reason})");
             }
        }

        /// <inheritdoc />
        public void OnComplete(int passed, int failed, int skipped, TimeSpan duration)
        {
             lock (_lock)
             {
                Console.WriteLine();
                
                var color = failed > 0 ? ConsoleColor.Red : ConsoleColor.Green;
                Console.ForegroundColor = color;
                Console.WriteLine("═══════════════════════════════════════════════");
                Console.WriteLine($"  Total:   {passed + failed + skipped}");
                Console.WriteLine($"  Passed:  {passed}");
                Console.WriteLine($"  Failed:  {failed}");
                Console.WriteLine($"  Skipped: {skipped}");
                Console.WriteLine($"  Time:    {duration.TotalSeconds:F3}s");
                Console.WriteLine("═══════════════════════════════════════════════");
                Console.ResetColor();
             }
            
            if (failed > 0)
            {
                Environment.ExitCode = 1;
            }
        }
    }
}
