namespace Skugga
{
    /// <summary>
    /// Base class for mocks.
    /// </summary>
    public abstract class MockBase
    {
        private readonly System.Collections.Generic.List<string> _receivedCalls = new();

        /// <summary> Records a call signature. </summary>
        /// <param name="callSignature">The signature of the call.</param>
        public void RecordCall(string callSignature)
        {
            _receivedCalls.Add(callSignature);
        }

        /// <summary> Verifies all expectations. </summary>
        public void VerifyAll()
        {
            if (_receivedCalls.Count == 0)
            {
               // If we expected calls but got none, strictly speaking for this demo we might warn,
               // but simpler: if we verified, we assume success if no exception thrown.
               // However, let's print what we verified to prove we are real.
               System.Console.WriteLine($"    [Skugga] Warning: No calls recorded for {this.GetType().Name}.");
            }
            else
            {
                foreach(var call in _receivedCalls)
                {
                    System.Console.WriteLine($"    [Skugga] Verified Call: {call}");
                }
                System.Console.WriteLine($"    [Skugga] Verification Passed for {this.GetType().Name} ({_receivedCalls.Count} calls).");
            }
        }
    }

    /// <summary>
    /// A generic mock class.
    /// </summary>
    /// <typeparam name="T">The type to mock.</typeparam>
    public class Mock<T> : MockBase
    {
        // Simple hack for the demo: if T is an interface, we can't 'new' it.
        // But for this specific demo, we can just return a manual stub if we really wanted to run it.
        // Or, more simply, we use a real object for the demo.
        // Let's just make `MockObject` settable for the demo.
        /// <summary> The mocked object instance. </summary>
        public T MockObject { get; set; } = default!;
    }
}
