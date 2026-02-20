using System.Threading.Tasks;
using Prova;
using Prova.Core;

namespace Prova.Core.Tests.Framework
{
    /// <summary>
    /// Tests for the EventRegistry.
    /// </summary>
    [DoNotParallelize]
    public class EventRegistryTests
    {
        /// <summary>
        /// Clears the registry before each test.
        /// </summary>
        [Before]
        public void Setup() => EventRegistry.Clear();

        /// <summary>Mock receiver for start events.</summary>
        private sealed class MockStartReceiver : ITestStartEventReceiver
        {
            public ProvaTest? LastTest { get; private set; }
            public int Calls { get; private set; }

            public Task OnTestStartAsync(ProvaTest test)
            {
                LastTest = test;
                Calls++;
                return Task.CompletedTask;
            }
        }

        /// <summary>Mock receiver for end events.</summary>
        private sealed class MockEndReceiver : ITestEndEventReceiver
        {
            public ProvaTest? LastTest { get; private set; }
            public TestResult LastResult { get; private set; }
            public long LastDuration { get; private set; }
            public int Calls { get; private set; }

            public Task OnTestEndAsync(ProvaTest test, TestResult result, long durationMs)
            {
                LastTest = test;
                LastResult = result;
                LastDuration = durationMs;
                Calls++;
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Verifies that the registry dispatches the start event.
        /// </summary>
        [Fact]
        public async Task Should_Dispatch_Start_Event()
        {
            // Arrange
            var receiver = new MockStartReceiver();
            EventRegistry.Register(receiver);
            var test = new ProvaTest { DisplayName = "Test1", ExecuteDelegate = () => Task.FromResult<string?>(null) };

            // Act
            await EventRegistry.DispatchStartAsync(test);

            // Assert
            Assert.Equal(1, receiver.Calls);
            Assert.Equal("Test1", receiver.LastTest?.DisplayName);
        }

        /// <summary>
        /// Verifies that the registry dispatches the end event.
        /// </summary>
        [Fact]
        public async Task Should_Dispatch_End_Event()
        {
            // Arrange
            var receiver = new MockEndReceiver();
            EventRegistry.Register(receiver);
            var test = new ProvaTest { DisplayName = "Test2", ExecuteDelegate = () => Task.FromResult<string?>(null) };

            // Act
            await EventRegistry.DispatchEndAsync(test, TestResult.Passed, 123);

            // Assert
            Assert.Equal(1, receiver.Calls);
            Assert.Equal("Test2", receiver.LastTest?.DisplayName);
            Assert.Equal(TestResult.Passed, receiver.LastResult);
            Assert.Equal(123, receiver.LastDuration);
        }

        /// <summary>
        /// Verifies that the registry handles multiple receivers.
        /// </summary>
        [Fact]
        public async Task Should_Handle_Multiple_Receivers()
        {
            // Arrange
            var receiver1 = new MockStartReceiver();
            var receiver2 = new MockStartReceiver();
            EventRegistry.Register(receiver1);
            EventRegistry.Register(receiver2);
            var test = new ProvaTest { DisplayName = "MultiTest", ExecuteDelegate = () => Task.FromResult<string?>(null) };

            // Act
            await EventRegistry.DispatchStartAsync(test);

            // Assert
            Assert.Equal(1, receiver1.Calls);
            Assert.Equal(1, receiver2.Calls);
        }
    }
}
