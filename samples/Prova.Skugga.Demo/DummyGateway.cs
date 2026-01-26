namespace Skugga
{
    public class DummyPaymentGateway : DemoApp.IPaymentGateway
    {
        private readonly System.Action<string> _recorder;

        public DummyPaymentGateway(System.Action<string> recorder)
        {
            _recorder = recorder;
        }

        public void Charge(decimal amount) 
        { 
            _recorder?.Invoke($"Charge({amount})");
        }
    }
}
