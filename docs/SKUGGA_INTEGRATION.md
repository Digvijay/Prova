# Skugga Integration: Smart Verify 🛡️

Prova features deep, zero-dependency integration with **Skugga**, the high-performance AOT mocking library. This integration is part of the **Nordic AOT Suite**.

## How "Smart Verify" Works

Prova's "Smart Verify" is a compiler-time optimization. It does **not** add a runtime dependency on Skugga. Instead, the Prova Source Generator uses heuristics to detect mocks and automate verification.

### 🛡️ No Hard Dependency
The Prova binaries do not reference Skugga. Prova remains a lightweight, standalone testing framework. The integration is activated only when it detects Skugga types in your test code.

### 🧙‍♂️ The "Magic" Logic
During compilation, the Prova generator:
1.  Scans for fields in your test class.
2.  Checks if a field type name contains `"Skugga"` or `"Mock<"`.
3.  If detected, Prova automatically injects a `.VerifyAll()` call at the end of every test method in that class.

## Quick Start Sample

To use Prova with Skugga, simply define your mocks as fields in your test class.

### 1. Install Dependencies
```bash
# Add Prova
dotnet add package Prova

# Add Skugga (v1.3.1+)
dotnet add package Skugga --version 1.3.1
```

### 2. Write an Automated Mock Test
```csharp
using Prova;
using Skugga;

public class PaymentTests
{
    // Prova detects this field and will call _gatewayMock.VerifyAll() automatically!
    internal readonly Mock<IPaymentGateway> _gatewayMock = new Mock<IPaymentGateway>();

    [Fact]
    public void Charge_CallsGateway()
    {
        var processor = new PaymentProcessor(_gatewayMock.Object);
        processor.Process(100m);
        
        // No manual _gatewayMock.VerifyAll() needed!
        // The Prova runner handles it for you.
    }
}
```

## Resources

- **Skugga Repository**: [https://github.com/Digvijay/Skugga](https://github.com/Digvijay/Skugga)
- **Skugga NuGet**: [https://www.nuget.org/packages/Skugga/1.3.1](https://www.nuget.org/packages/Skugga/1.3.1)

---
*Part of the Nordic AOT Suite: Building the future of zero-reflection .NET.*
