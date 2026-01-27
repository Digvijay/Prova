namespace Skugga
{
    /// <summary>
    /// A dummy payment gateway for testing purposes.
    /// </summary>
    public class DummyPaymentGateway : DemoApp.IPaymentGateway
    {
        private readonly System.Action<string> _recorder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyPaymentGateway"/> class.
        /// </summary>
        /// <param name="recorder">Action to record calls.</param>
        public DummyPaymentGateway(System.Action<string> recorder)
        {
            _recorder = recorder;
        }

        /// <inheritdoc />
        public void Charge(decimal amount) 
        { 
            _recorder?.Invoke($"Charge({amount})");
        }
    }
}
