using System;
using System.IO;
using System.Text;
using Prova;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Tests for the CoverageRegistry and LCOV emission.
    /// </summary>
    public class CoverageRegistryTests
    {
        private static readonly string[] TestMetadata = new[] { "Test" };

        /// <summary>
        /// Verifies that Initialize sets up the registry correctly.
        /// </summary>
        [Fact]
        public void Initialize_Sets_Up_Registry()
        {
            // Arrange
            var metadata = new string[] { "Test1", "Test2", "Test3" };

            // Act
            CoverageRegistry.Initialize(3, metadata);

            // Assert - no exceptions means success
            // Registry is internal, so we just verify the setup doesn't throw
        }

        /// <summary>
        /// Verifies that Hit records execution in the registry.
        /// </summary>
        [Fact]
        public void Hit_Records_Execution()
        {
            // Arrange
            var metadata = new string[] { "TestMethod" };
            CoverageRegistry.Initialize(1, metadata);

            // Act
            CoverageRegistry.Hit(0);

            // Assert - verify via LCOV output
            var tempPath = Path.GetTempFileName();
            try
            {
                CoverageRegistry.EmitLcov(tempPath);
                var content = File.ReadAllText(tempPath);

                Assert.Contains("DA:1,1", content); // Line 1 hit once
                Assert.Contains("LH:1", content);   // 1 line hit
                Assert.Contains("LF:1", content);   // 1 line found
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }

        /// <summary>
        /// Verifies that EmitLcov generates a valid LCOV format.
        /// </summary>
        [Fact]
        public void EmitLcov_Generates_Valid_Format()
        {
            // Arrange
            var metadata = new string[] { "Class.Method1", "Class.Method2" };
            CoverageRegistry.Initialize(2, metadata);
            CoverageRegistry.Hit(0); // Hit first method

            // Act
            var tempPath = Path.GetTempFileName();
            try
            {
                CoverageRegistry.EmitLcov(tempPath);
                var content = File.ReadAllText(tempPath);

                // Assert LCOV format
                Assert.Contains("SF:", content);           // Source file
                Assert.Contains("DA:", content);           // Data lines
                Assert.Contains("LF:", content);           // Lines found
                Assert.Contains("LH:", content);           // Lines hit
                Assert.Contains("end_of_record", content); // End marker
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }

        /// <summary>
        /// Verifies that Hit with an invalid ID doesn't throw an exception.
        /// </summary>
        [Fact]
        public void Hit_With_Invalid_Id_Does_Not_Throw()
        {
            // Arrange
            CoverageRegistry.Initialize(1, TestMetadata);

            // Act & Assert - should not throw
            CoverageRegistry.Hit(-1);
            CoverageRegistry.Hit(100);
        }
    }
}
