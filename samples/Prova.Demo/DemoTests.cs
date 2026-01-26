using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Prova;

namespace Prova.Demo
{
    // 1. Basic Tests
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

    // 2. Data Driven Tests (MemberData & InlineData)
    public class DataTests
    {
        [Theory]
        [InlineData(10, 2, 5)]
        [InlineData(100, 10, 10)]
        public static void Division(int a, int b, int expected)
        {
            Assert.Equal(expected, a / b);
        }

        public static IEnumerable<object[]> GetNames()
        {
            yield return new object[] { "Prova" };
            yield return new object[] { "xUnit" };
        }

        [Theory]
        [MemberData(nameof(GetNames))]
        public static void OutputNames(string name)
        {
            Assert.Contains("Unit", name);
        }
    }

    // 3. Dependency Injection (Fixtures)
    public class DatabaseFixture : IAsyncLifetime
    {
        public bool IsConnected { get; private set; }

        public Task InitializeAsync()
        {
            IsConnected = true;
            Console.WriteLine("Database Connected 🔌");
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            IsConnected = false;
            Console.WriteLine("Database Disconnected 🔌");
            return Task.CompletedTask;
        }
    }

    public class DatabaseTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _db;

        public DatabaseTests(DatabaseFixture db)
        {
            _db = db;
        }

        [Fact]
        public void ConnectionIsValid()
        {
            Assert.True(_db.IsConnected);
        }
    }

    // 4. Developer Experience (Retry)
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

    // 5. Traits
    [Trait("Category", "Slow")]
    public class SlowTests
    {
        [Fact]
        public static async Task HeavyWork()
        {
            await Task.Delay(100);
        }
    }
}
