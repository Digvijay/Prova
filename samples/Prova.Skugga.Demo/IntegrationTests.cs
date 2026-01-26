using System;
using System.Threading.Tasks;
using Prova;
using Skugga;

namespace DemoApp
{
    public interface IPaymentGateway
    {
        void Charge(decimal amount);
    }

    public class PaymentProcessor
    {
        private readonly IPaymentGateway _gateway;
        public PaymentProcessor(IPaymentGateway gateway) => _gateway = gateway;

        public void Process(decimal amount) => _gateway.Charge(amount);
    }

    public class PaymentTests
    {
        // gUnit/Prova should automatically detect this field is a Mock
        // and call _gatewayMock.VerifyAll() at the end of the test!
        internal readonly Mock<IPaymentGateway> _gatewayMock = new Mock<IPaymentGateway>();

        public PaymentTests()
        {
            // Manual stubbing for the demo since we don't have a real proxy generator here
            _gatewayMock.Object = new Skugga.DummyPaymentGateway((call) => _gatewayMock.RecordCall(call)); 
        }

        /// <summary>
        /// ⚡ Charging through the Nordic Gateway 🌩️
        /// </summary>
        [Fact]
        public void Charge_DelegatesToGateway()
        {
            var processor = new PaymentProcessor(_gatewayMock.Object);
            processor.Process(100m);
            
            // NO MANUAL VERIFY CALL NEEDED! 
            // _gatewayMock.VerifyAll() is injected by the compiler.
        }
    }
}
