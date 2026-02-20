# Hybrid MTP Adapter

Prova implements a **Hybrid Adapter** for the **Microsoft Testing Platform (MTP)**. This allows Prova to provide its own high-performance AOT engine while still participating in the broader .NET testing ecosystem.

## How it Works

Unlike traditional adapters that are discovered via reflection at runtime, Prova's MTP adapter is **Source Generated**.

1. **Source Generation**: Prova generates an MTP `ITestApplication` entry point.
2. **Registration**: Your tests are registered as MTP-compatible tests.
3. **Execution**: When you run `dotnet test`, MTP initializes Prova's engine.

## Why Hybrid?

- **AOT Performance**: Standard MTP adapters often rely on reflection or dynamic loading, which are slow or incompatible with Native AOT.
- **Ecosystem Support**: By supporting MTP, Prova can output **TRX** files, integrate with Visual Studio's Test Explorer (experimental), and work with CI reporters.

## Usage

To enable MTP support, ensure you have the `Microsoft.Testing.Platform.MSBuild` package in your project.

```xml
<PackageReference Include="Microsoft.Testing.Platform.MSBuild" Version="1.0.0" />
```

### Running with MTP Features

You can use standard MTP arguments:

```bash
# Generate a TRX report
dotnet test -- --report-trx

# Filter tests using MTP syntax
dotnet test -- --filter "Namespace=CalculatorTests"
```

## Future Road Map

We are working on deeper integration to support:
- Hot Reload
- Improved VS Test Explorer integration
- Custom MTP extensions for Prova-specific features (like allocation tracking)
