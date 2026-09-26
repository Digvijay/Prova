using System;

namespace Prova.Logging
{
    /// <summary>
    /// The build host whose annotation syntax <see cref="ConsoleLogger"/> should emit.
    /// </summary>
    public enum ConsoleLogHost
    {
        /// <summary>A plain terminal: human-readable prefixes and colour.</summary>
        Plain = 0,

        /// <summary>GitHub Actions: <c>::warning::</c> and <c>::error::</c> workflow commands.</summary>
        GitHubActions = 1,

        /// <summary>Azure Pipelines: <c>##vso[task.logissue]</c> logging commands.</summary>
        AzureDevOps = 2,
    }

    /// <summary>
    /// A logger that writes to the console.
    /// </summary>
    public class ConsoleLogger : ITestLogger
    {
        private readonly ConsoleLogHost _host;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class, detecting the
        /// build host from the environment.
        /// </summary>
        public ConsoleLogger()
            : this(DetectHost())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class for a specific host.
        /// </summary>
        /// <param name="host">The annotation syntax to emit.</param>
        /// <remarks>
        /// The parameterless constructor reads ambient environment variables, so its behaviour
        /// changes depending on where the process runs. Anything that needs to assert on the
        /// output — including this library's own tests — should name the host instead.
        /// </remarks>
        public ConsoleLogger(ConsoleLogHost host)
        {
            _host = host;
        }

        /// <summary>
        /// Determines the build host from the ambient environment.
        /// </summary>
        /// <returns>The detected host, or <see cref="ConsoleLogHost.Plain"/>.</returns>
        public static ConsoleLogHost DetectHost()
        {
            if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
            {
                return ConsoleLogHost.GitHubActions;
            }

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")))
            {
                return ConsoleLogHost.AzureDevOps;
            }

            return ConsoleLogHost.Plain;
        }

        /// <inheritdoc />
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }

        /// <inheritdoc />
        public void LogWarning(string message)
        {
            switch (_host)
            {
                case ConsoleLogHost.GitHubActions:
                    Console.WriteLine($"::warning::{message}");
                    break;
                case ConsoleLogHost.AzureDevOps:
                    Console.WriteLine($"##vso[task.logissue type=warning]{message}");
                    break;
                default:
                    WriteInColour(ConsoleColor.Yellow, $"[WARN] {message}");
                    break;
            }
        }

        /// <inheritdoc />
        public void LogError(string message)
        {
            switch (_host)
            {
                case ConsoleLogHost.GitHubActions:
                    Console.WriteLine($"::error::{message}");
                    break;
                case ConsoleLogHost.AzureDevOps:
                    Console.WriteLine($"##vso[task.logissue type=error]{message}");
                    break;
                default:
                    WriteInColour(ConsoleColor.Red, $"[ERR] {message}");
                    break;
            }
        }

        private static void WriteInColour(ConsoleColor colour, string line)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.WriteLine(line);
            Console.ForegroundColor = original;
        }
    }
}
