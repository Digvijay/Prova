# Skugga Integration: Automated Mock Verification

Prova supports optional integration with **Skugga**, an AOT mocking library. This integration allows for automated mock verification without adding a hard runtime dependency.

## Mock Verification Automation

Prova uses compile-time heuristics to detect mock objects within test classes and automate their verification lifecycle.

### Loose Coupling
The Prova binaries do not reference Skugga. The integration is activated via source generation only when Skugga types are detected in the user's codebase.

### Detection Logic
During the source generation phase, Prova:
1.  Scans for fields within the test class.
2.  Checks if a field's type name contains `"Skugga"` or `"Mock<"`.
3.  If detected, Prova generates a `finally` block that invokes `.VerifyAll()` on the mock instance at the end of each test method execution.

## Usage Example

To enable automated verification, define mocks as fields in the test class.

### 1. Install Dependencies
```bash
dotnet add package Prova
dotnet add package Skugga
```

### 2. Implementation
```csharp
using Prova;
using Skugga;

public class PaymentTests
{
    // Prova detects this field and generates a call to _paymentGateway.VerifyAll()
    // at the end of every test method in this class.
    private readonly Mock<IPaymentGateway> _paymentGateway = new Mock<IPaymentGateway>();

    [Fact]
    public void Charge_CallsGateway()
    {
        var processor = new PaymentProcessor(_paymentGateway.Object);
        processor.Process(100m);
        
        // Manual verification is not required.
        // If expectation was not met, the test will fail during teardown.
    }
}
```

## Resources

- **Skugga Repository**: [https://github.com/Digvijay/Skugga](https://github.com/Digvijay/Skugga)
