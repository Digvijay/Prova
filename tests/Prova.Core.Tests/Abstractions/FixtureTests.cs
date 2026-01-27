using System;
using Prova;

namespace Prova.Core.Tests
{
    /// <summary>
    /// A database fixture for testing class fixtures.
    /// </summary>
    public class DatabaseFixture : IDisposable
    {
        /// <summary>Gets a value indicating whether the fixture is initialized.</summary>
        public bool IsInitialized { get; }

        /// <summary>Initializes a new instance of the <see cref="DatabaseFixture"/> class.</summary>
        public DatabaseFixture()
        {
            IsInitialized = true;
            Console.WriteLine("DatabaseFixture Created");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Console.WriteLine("DatabaseFixture Disposed");
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Tests creating and using class fixtures.
    /// </summary>
    public class FixtureTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly ITestOutputHelper _output;

        /// <summary>Initializes a new instance of the <see cref="FixtureTests"/> class.</summary>
        public FixtureTests(DatabaseFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        /// <summary>Verifies fixture initialization.</summary>
        [Fact]
        public void FixtureIsInitialized()
        {
            Assert.True(_fixture.IsInitialized);
            _output.WriteLine("Fixture check passed!");
        }

        /// <summary>Verifies output capture.</summary>
        [Fact]
        public void OutputIsCaptured()
        {
            _output.WriteLine("This is captured output from a passing test.");
            Assert.Equal(1, 1);
        }
        
        /// <summary>Verifies output capture on failure.</summary>
        [Fact(Skip = "Intentional failure for framework verification")]
        public void OutputCapturedOnFailure()
        {
            _output.WriteLine("This is captured output from a FAILING test.");
            Assert.True(false, "Intentional Failure");
        }
    }
}
