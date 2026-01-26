using System;
using Prova;

namespace Prova.Core.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public bool IsInitialized { get; }

        public DatabaseFixture()
        {
            IsInitialized = true;
            Console.WriteLine("DatabaseFixture Created");
        }

        public void Dispose()
        {
            Console.WriteLine("DatabaseFixture Disposed");
            GC.SuppressFinalize(this);
        }
    }

    public class FixtureTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly ITestOutputHelper _output;

        public FixtureTests(DatabaseFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact]
        public void FixtureIsInitialized()
        {
            Assert.True(_fixture.IsInitialized);
            _output.WriteLine("Fixture check passed!");
        }

        [Fact]
        public void OutputIsCaptured()
        {
            _output.WriteLine("This is captured output from a passing test.");
            Assert.Equal(1, 1);
        }
        
        [Fact(Skip = "Intentional failure for framework verification")]
        public void OutputCapturedOnFailure()
        {
            _output.WriteLine("This is captured output from a FAILING test.");
            Assert.True(false, "Intentional Failure");
        }
    }
}
