using System;
using System.IO;
using Prova.Logging;

namespace Prova.Core.Tests.Logging
{
    /// <summary>Tests for the ConsoleLogger.</summary>
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
