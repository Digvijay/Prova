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
        <PackageReference Include="Prova" Version="0.4.0" />
    </ItemGroup>

</Project>
```

## Writing Your First Test

Prova uses attributes you are likely familiar with (`[Fact]`, `[Theory]`) but lives in the `Prova` namespace.

```csharp
using Prova;
using Prova.Assertions;

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

Since your test project is a standard console app, you can run it directly or use `dotnet test`.

```bash
dotnet test
```

Or run the executable directly for instant feedback:

```bash
# Faster run without MSBuild overhead
./bin/Debug/net10.0/MyTestProject
```

## Next Steps

- **[Migrating from xUnit](./migration.md)**: Learn how to use our automatic code fixers.
- **[Assertions](../api/assertions.md)**: Explore the full assertion library.
- **[Architecture](../concepts/architecture.md)**: Understand why we chose Native AOT.
