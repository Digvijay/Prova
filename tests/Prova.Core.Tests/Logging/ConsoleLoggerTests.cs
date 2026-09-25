using System;
using System.IO;
using Prova.Logging;

namespace Prova.Core.Tests.Logging
{
    /// <summary>Tests for the ConsoleLogger.</summary>
    /// <remarks>
    /// Every test here swaps <see cref="Console.Out"/>, which is process-global and is also
    /// written to by the test runner itself while other tests execute. Sharing a resource key
    /// only serialises these three tests against each other, so full exclusivity is used to
    /// keep the captured output attributable to the logger call under test.
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
            // Arrange
            var logger = new ConsoleLogger();
            var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);

            try
            {
                // Act
                logger.Log("test message");

                // Assert
                var output = writer.ToString();
                Assert.Contains("[LOG] test message", output);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        /// <summary>
        /// Verifies that LogWarning writes to the console with the correct prefix.
        /// </summary>
        [Fact]
        public void LogWarning_WritesToConsole_WithWarnPrefix()
        {
            // Arrange
            var logger = new ConsoleLogger();
            var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);

            try
            {
                // Act
                logger.LogWarning("warning message");

                // Assert
                var output = writer.ToString();
                Assert.Contains("[WARN] warning message", output);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        /// <summary>
        /// Verifies that LogError writes to the console with the correct prefix.
        /// </summary>
        [Fact]
        public void LogError_WritesToConsole_WithErrPrefix()
        {
            // Arrange
            var logger = new ConsoleLogger();
            var writer = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(writer);

            try
            {
                // Act
                logger.LogError("error message");

                // Assert
                var output = writer.ToString();
                Assert.Contains("[ERR] error message", output);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
