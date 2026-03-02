# Getting Started

> [!TIP]
> Prova is designed to be a drop-in replacement for xUnit but built for the modern Native AOT era.

## Installation

### 1. Create a Console Project
Tests in Prova are executable console applications. This gives you full control over the entry point and configuration.

```bash
dotnet new console -n MyTestProject
cd MyTestProject
```

### 2. Install Prova
Add the Prova package to your project.

```bash
dotnet add package Prova
```

### 3. Configure `.csproj`
Enable **Native AOT** and ensure the output type is `Exe`.

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <!-- Crucial: Enable Native AOT -->
        <PublishAot>true</PublishAot>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Prova" Version="0.5.0" />
    </ItemGroup>

</Project>
```

> [!NOTE]
> Prova auto-generates a `Program.cs` entry point for you. If your project has a custom `Program.cs` you can delete it, or disable auto-generation by setting `<GenerateProgramFile>false</GenerateProgramFile>` in your `.csproj`.

## Writing Your First Test

Prova uses attributes you are likely familiar with (`[Fact]`, `[Theory]`) but lives in the `Prova` namespace.

```csharp
using Prova;
using Prova.Assertions; // Working as of v0.5.0!

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        int result = 2 + 2;
        Assert.Equal(4, result);
    }

    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(5, 5, 10)]
    public void Add_WithParams_ReturnsSum(int a, int b, int expected)
    {
        Assert.Equal(expected, a + b);
    }
}
```

## Running Tests

Prova v0.5.0 runs natively on the Microsoft Testing Platform (MTP). You can run tests via standard CLI commands without any VSTest bridge:

```bash
# Native Microsoft Testing Platform features
dotnet test

# Run as a self-executing console app for instant feedback
dotnet run
```

## Next Steps

- **[Migrating from xUnit](./migration.md)**: Learn how to use our automatic code fixers.
- **[Assertions](../api/assertions.md)**: Explore the full assertion library.
- **[Architecture](../concepts/architecture.md)**: Understand why we chose Native AOT.
