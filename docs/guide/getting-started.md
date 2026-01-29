# Getting Started

## Installation

### Manually
First, create a console application (Tests in Prova are executables).

```bash
dotnet new console -n MyTestProject
cd MyTestProject
```

To that project, add the **Prova** package:

```bash
dotnet add package Prova
```

### Project Configuration
Update your `.csproj` to enable **Native AOT** and standard testing capabilities.

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <PublishAot>true</PublishAot> <!-- Crucial for Prova -->
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Prova" Version="0.3.0" />
        <PackageReference Include="Microsoft.Testing.Platform" Version="1.0.0" />
    </ItemGroup>

</Project>
```

### Global Usings
Prova works seamlessly with global usings to keep your test files clean.

```csharp
// GlobalUsings.cs
global using Prova;
global using Prova.Assertions;
```

## Writing Your First Test

Prova is compatible with standard xUnit attributes.

```csharp
using xUnit;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        Assert.Equal(4, 2 + 2);
    }
}
```

## Running Tests

Run your tests using the standard `dotnet test` command.

```bash
dotnet test
```
