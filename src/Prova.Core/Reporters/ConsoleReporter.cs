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
                Console.Write("[PASS] ");
                Console.ResetColor();
                Console.WriteLine(testName);
                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine("Output:");
                    Console.WriteLine(output);
                }
            }
        }

        /// <inheritdoc />
        public void OnTestFailure(string testName, Exception ex, string output)
        {
             lock (_lock)
             {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[FAIL] ");
                Console.ResetColor();
                Console.WriteLine($"{testName}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine("Output:");
                    Console.WriteLine(output);
                }
             }
        }

        /// <inheritdoc />
        public void OnTestSkipped(string testName, string reason)
        {
             lock (_lock)
             {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[SKIP] ");
                Console.ResetColor();
                Console.WriteLine($"{testName}: {reason}");
             }
        }

        /// <inheritdoc />
        public void OnComplete(int passed, int failed, int skipped, TimeSpan duration)
        {
             lock (_lock)
             {
                Console.WriteLine();
                Console.WriteLine($"Total: {passed + failed + skipped}, Passed: {passed}, Failed: {failed}, Skipped: {skipped}");
                Console.WriteLine($"Duration: {duration.TotalMilliseconds}ms");
             }
            
            if (failed > 0)
            {
                Environment.ExitCode = 1;
            }
        }
    }
}
