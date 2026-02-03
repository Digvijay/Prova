using System;

namespace Prova.Logging
{
    /// <summary>
    /// A logger that writes to the console.
    /// </summary>
    public class ConsoleLogger : ITestLogger
    {
        private readonly bool _isGitHubActions;
        private readonly bool _isAzureDevOps;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        public ConsoleLogger()
        {
            _isGitHubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
            _isAzureDevOps = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD"));
        }

        /// <inheritdoc />
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }

        /// <inheritdoc />
        public void LogWarning(string message)
        {
            if (_isGitHubActions)
            {
                // GitHub Actions Warning Syntax: ::warning::{message}
                Console.WriteLine($"::warning::{message}");
            }
            else if (_isAzureDevOps)
            {
                // Azure DevOps Warning Syntax: ##vso[task.logissue type=warning]{message}
                Console.WriteLine($"##vso[task.logissue type=warning]{message}");
            }
            else
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARN] {message}");
                Console.ForegroundColor = color;
            }
        }

        /// <inheritdoc />
        public void LogError(string message)
        {
             if (_isGitHubActions)
            {
                // GitHub Actions Error Syntax: ::error::{message}
                Console.WriteLine($"::error::{message}");
            }
            else if (_isAzureDevOps)
            {
                // Azure DevOps Error Syntax: ##vso[task.logissue type=error]{message}
                Console.WriteLine($"##vso[task.logissue type=error]{message}");
            }
            else
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERR] {message}");
                Console.ForegroundColor = color;
            }
        }
    }
}
