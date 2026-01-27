using System;
using System.Threading.Tasks;
using Prova;
using Skugga;

namespace DemoApp
{
    /// <summary>
    /// Represents a payment gateway interface.
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>Charges the specified amount.</summary>
        /// <param name="amount">The amount to charge.</param>
        void Charge(decimal amount);
    }

    /// <summary>
    /// Processes payments using a gateway.
    /// </summary>
    public class PaymentProcessor
    {
        private readonly IPaymentGateway _gateway;
        
        /// <summary>Initializes a new instance of the <see cref="PaymentProcessor"/> class.</summary>
        /// <param name="gateway">The payment gateway to use.</param>
        public PaymentProcessor(IPaymentGateway gateway) => _gateway = gateway;

        /// <summary>Processes the payment.</summary>
        /// <param name="amount">The amount to process.</param>
        public void Process(decimal amount) => _gateway.Charge(amount);
    }

    /// <summary>
    /// Tests for the payment processor.
    /// </summary>
    public class PaymentTests
    {
        // gUnit/Prova should automatically detect this field is a Mock
        // and call _gatewayMock.VerifyAll() at the end of the test!
        internal readonly Mock<IPaymentGateway> _gatewayMock = new Mock<IPaymentGateway>();

        /// <summary>Initializes a new instance of the <see cref="PaymentTests"/> class.</summary>
        public PaymentTests()
        {
            // Manual stubbing for the demo since we don't have a real proxy generator here
            _gatewayMock.MockObject = new Skugga.DummyPaymentGateway((call) => _gatewayMock.RecordCall(call)); 
        }

        /// <summary>
        /// ⚡ Charging through the Nordic Gateway 🌩️
        /// </summary>
        [Fact]
        public void ChargeDelegatesToGateway()
        {
            var processor = new PaymentProcessor(_gatewayMock.MockObject);
            processor.Process(100m);
            
            // NO MANUAL VERIFY CALL NEEDED! 
            // _gatewayMock.VerifyAll() is injected by the compiler.
        }
    }
}
