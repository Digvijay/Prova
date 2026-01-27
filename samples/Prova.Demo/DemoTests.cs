using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Prova;

namespace Prova.Demo
{
    /// <summary>
    /// Basic Tests
    /// </summary>
    public class BasicTests
    {
        /// <summary>
        /// Testing that 1+1 is indeed 2
        /// </summary>
        [Fact]
        public static void Addition()
        {
            Assert.Equal(2, 1 + 1);
        }

        /// <summary>
        /// This test will run in parallel with others! ⚡
        /// </summary>
        [Fact]
        public static async Task AsyncDelay()
        {
            await Task.Delay(500);
            Assert.True(true);
        }
    }

    /// <summary>
    /// Data Driven Tests (MemberData &amp; InlineData)
    /// </summary>
    public class DataTests
    {
        /// <summary>Test division.</summary>
        /// <param name="a">Dividend.</param>
        /// <param name="b">Divisor.</param>
        /// <param name="expected">Expected quotient.</param>
        [Theory]
        [InlineData(10, 2, 5)]
        [InlineData(100, 10, 10)]
        public static void Division(int a, int b, int expected)
        {
            Assert.Equal(expected, a / b);
        }

        /// <summary>Gets test names.</summary>
        public static IEnumerable<object[]> GetNames()
        {
            yield return new object[] { "Prova" };
            yield return new object[] { "xUnit" };
        }

        /// <summary>Check output names.</summary>
        /// <param name="name">The name.</param>
        [Theory]
        [MemberData(nameof(GetNames))]
        public static void OutputNames(string name)
        {
            Assert.Contains("Unit", name);
        }
    }

    /// <summary>
    /// Dependency Injection (Fixtures)
    /// </summary>
    public class DatabaseFixture : IAsyncLifetime
    {
        /// <summary>Is connected status.</summary>
        public bool IsConnected { get; private set; }

        /// <inheritdoc />
        public Task InitializeAsync()
        {
            IsConnected = true;
            Console.WriteLine("Database Connected 🔌");
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DisposeAsync()
        {
            IsConnected = false;
            Console.WriteLine("Database Disconnected 🔌");
            return Task.CompletedTask;
        }
    }

    /// <summary>DB Tests.</summary>
    public class DatabaseTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _db;

        /// <summary>Constructor.</summary>
        public DatabaseTests(DatabaseFixture db)
        {
            _db = db;
        }

        /// <summary>Check connection.</summary>
        [Fact]
        public void ConnectionIsValid()
        {
            Assert.True(_db.IsConnected);
        }
    }

    /// <summary>
    /// Developer Experience (Retry)
    /// </summary>
    public class FlakyServiceTests
    {
        private static int _attempts;

        /// <summary>
        /// This test pretends to be flaky (e.g. network timeout).
        /// Prova retries it up to 3 times automatically! 🛡️
        /// </summary>
        [Fact]
        [Retry(3)]
        public static void UnstableTest()
        {
            _attempts++;
            if (_attempts < 2)
            {
                throw new InvalidOperationException("Network glitch! ⚡");
            }
            Assert.True(true);
        }
    }

    /// <summary>
    /// Traits
    /// </summary>
    [Trait("Category", "Slow")]
    public class SlowTests
    {
        /// <summary>Heavy work.</summary>
        [Fact]
        public static async Task HeavyWork()
        {
            await Task.Delay(100);
        }
    }
}
