using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prova
{
    /// <summary>
    /// Manages the registration and dispatching of test lifecycle events.
    /// </summary>
    public static class EventRegistry
    {
        private static readonly List<ITestStartEventReceiver> _startReceivers = new();
        private static readonly List<ITestEndEventReceiver> _endReceivers = new();
        private static readonly object _lock = new();

        /// <summary>
        /// Registers a new event receiver.
        /// </summary>
        public static void Register(ITestEventReceiver receiver)
        {
            lock (_lock)
            {
                if (receiver is ITestStartEventReceiver start)
                    _startReceivers.Add(start);
                
                if (receiver is ITestEndEventReceiver end)
                    _endReceivers.Add(end);
            }
        }

        /// <summary>
        /// Unregisters an event receiver.
        /// </summary>
        public static void Unregister(ITestEventReceiver receiver)
        {
            lock (_lock)
            {
                if (receiver is ITestStartEventReceiver start)
                    _startReceivers.Remove(start);
                
                if (receiver is ITestEndEventReceiver end)
                    _endReceivers.Remove(end);
            }
        }

        /// <summary>
        /// Clears all registered receivers.
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _startReceivers.Clear();
                _endReceivers.Clear();
            }
        }

        /// <summary>
        /// Dispatches the TestStart event to all registered receivers.
        /// </summary>
        public static async Task DispatchStartAsync(ProvaTest test)
        {
            ITestStartEventReceiver[] receivers;
            lock (_lock) receivers = _startReceivers.ToArray();

            foreach (var receiver in receivers)
            {
                try
                {
                    await receiver.OnTestStartAsync(test);
                }
                catch
                {
                    // Events should not break test execution
                }
            }
        }

        /// <summary>
        /// Dispatches the TestEnd event to all registered receivers.
        /// </summary>
        public static async Task DispatchEndAsync(ProvaTest test, TestResult result, long durationMs)
        {
            ITestEndEventReceiver[] receivers;
            lock (_lock) receivers = _endReceivers.ToArray();

            foreach (var receiver in receivers)
            {
                try
                {
                    await receiver.OnTestEndAsync(test, result, durationMs);
                }
                catch
                {
                    // Events should not break test execution
                }
            }
        }
    }
}
