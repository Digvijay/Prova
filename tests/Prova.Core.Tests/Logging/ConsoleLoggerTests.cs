using System;
using System.IO;
using Prova.Logging;

namespace Prova.Core.Tests.Logging
{
    /// <summary>Tests for the ConsoleLogger.</summary>
    /// <remarks>
    /// <para>
    /// Every test here swaps <see cref="Console.Out"/>, which is process-global and is also
    /// written to by the test runner itself while other tests execute. Sharing a resource key
    /// only serialises these tests against each other, so full exclusivity is used to keep the
    /// captured output attributable to the logger call under test.
    /// </para>
    /// <para>
    /// These tests previously constructed <c>new ConsoleLogger()</c>, which detects the build
    /// host from ambient environment variables, and then asserted the plain-terminal output.
    /// That passes on a developer machine and fails on GitHub Actions, where the logger
    /// correctly emits <c>::error::</c> workflow commands instead — which is exactly what
    /// happened the first time CI ran. The host is now named explicitly, so each test asserts
    /// one defined behaviour and none of them depends on where the process runs.
    /// </para>
    /// </remarks>
    [DoNotParallelize]
    public sealed class ConsoleLoggerTests
    {
        /// <summary>
        /// Verifies that Log writes to the console with the correct prefix.
        /// </summary>
        [Fact]
        public void Log_WritesToConsole_WithPrefix()
        {
            var output = Capture(logger => logger.Log("test message"), ConsoleLogHost.Plain);

            Assert.Contains("[LOG] test message", output);
        }

        /// <summary>
        /// Verifies that LogWarning writes a human-readable prefix on a plain terminal.
        /// </summary>
        [Fact]
        public void LogWarning_OnAPlainTerminal_WritesTheWarnPrefix()
        {
            var output = Capture(logger => logger.LogWarning("warning message"), ConsoleLogHost.Plain);

            Assert.Contains("[WARN] warning message", output);
        }

        /// <summary>
        /// Verifies that LogError writes a human-readable prefix on a plain terminal.
        /// </summary>
        [Fact]
        public void LogError_OnAPlainTerminal_WritesTheErrPrefix()
        {
            var output = Capture(logger => logger.LogError("error message"), ConsoleLogHost.Plain);

            Assert.Contains("[ERR] error message", output);
        }

        /// <summary>
        /// Verifies that LogWarning emits a GitHub Actions workflow command.
        /// </summary>
        [Fact]
        public void LogWarning_OnGitHubActions_WritesAWorkflowCommand()
        {
            var output = Capture(logger => logger.LogWarning("warning message"), ConsoleLogHost.GitHubActions);

            Assert.Contains("::warning::warning message", output);
        }

        /// <summary>
        /// Verifies that LogError emits a GitHub Actions workflow command.
        /// </summary>
        [Fact]
        public void LogError_OnGitHubActions_WritesAWorkflowCommand()
        {
            var output = Capture(logger => logger.LogError("error message"), ConsoleLogHost.GitHubActions);

            Assert.Contains("::error::error message", output);
        }

        /// <summary>
        /// Verifies that LogWarning emits an Azure Pipelines logging command.
        /// </summary>
        [Fact]
        public void LogWarning_OnAzureDevOps_WritesALoggingCommand()
        {
            var output = Capture(logger => logger.LogWarning("warning message"), ConsoleLogHost.AzureDevOps);

            Assert.Contains("##vso[task.logissue type=warning]warning message", output);
        }

        /// <summary>
        /// Verifies that LogError emits an Azure Pipelines logging command.
        /// </summary>
        [Fact]
        public void LogError_OnAzureDevOps_WritesALoggingCommand()
        {
            var output = Capture(logger => logger.LogError("error message"), ConsoleLogHost.AzureDevOps);

            Assert.Contains("##vso[task.logissue type=error]error message", output);
        }

        /// <summary>
        /// Verifies that host detection reads the documented environment variables.
        /// </summary>
        [Fact]
        public void DetectHost_ReadsTheDocumentedEnvironmentVariables()
        {
            var actions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS");
            var azure = Environment.GetEnvironmentVariable("TF_BUILD");

            try
            {
                Environment.SetEnvironmentVariable("GITHUB_ACTIONS", null);
                Environment.SetEnvironmentVariable("TF_BUILD", null);
                Assert.Equal(ConsoleLogHost.Plain, ConsoleLogger.DetectHost());

                Environment.SetEnvironmentVariable("TF_BUILD", "True");
                Assert.Equal(ConsoleLogHost.AzureDevOps, ConsoleLogger.DetectHost());

                // GitHub Actions wins when both are set, matching the original ordering.
                Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
                Assert.Equal(ConsoleLogHost.GitHubActions, ConsoleLogger.DetectHost());
            }
            finally
            {
                Environment.SetEnvironmentVariable("GITHUB_ACTIONS", actions);
                Environment.SetEnvironmentVariable("TF_BUILD", azure);
            }
        }

        private static string Capture(Action<ConsoleLogger> act, ConsoleLogHost host)
        {
            var logger = new ConsoleLogger(host);
            var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);

            try
            {
                act(logger);
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            return writer.ToString();
        }
    }
}
