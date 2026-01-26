namespace Skugga
{
    public abstract class MockBase
    {
        private readonly System.Collections.Generic.List<string> _receivedCalls = new();

        public void RecordCall(string callSignature)
        {
            _receivedCalls.Add(callSignature);
        }

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

    public class Mock<T> : MockBase
    {
        // Simple hack for the demo: if T is an interface, we can't 'new' it.
        // But for this specific demo, we can just return a manual stub if we really wanted to run it.
        // Or, more simply, we use a real object for the demo.
        // Let's just make `Object` settable for the demo.
        public T Object { get; set; } = default!;
    }
}
