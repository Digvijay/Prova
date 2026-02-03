# Installation

Prova is distributed as a standard NuGet package. However, since it utilizes Native AOT for execution, the project setup is slightly different from a standard class library.

## Prerequisites

- **.NET 8 SDK** (or later)
- Platform-native toolchain (e.g., clang/gcc on Linux/Mac, MSVC on Windows) for AOT compilation.

## CLI Installation

The fastest way to get started is using the dotnet CLI.

### 1. Create a Console Application
Prova tests run as an executable console app, not a class library.

```bash
dotnet new console -n Prova.Tests
cd Prova.Tests
```

### 2. Add the Package
Install the latest version of Prova.

```bash
dotnet add package Prova
```

### 3. Configure for AOT
Open your `.csproj` file and ensure the following properties are set. This is critical for Prova to function correctly.

```xml
<PropertyGroup>
    <OutputType>Exe</OutputType>
    <PublishAot>true</PublishAot>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
</PropertyGroup>
```

## Package Reference

If you prefer editing the `.csproj` file manually, add the following reference:

```xml
<ItemGroup>
    <PackageReference Include="Prova" Version="0.3.0" />
    <PackageReference Include="Microsoft.Testing.Platform.MSBuild" Version="1.0.0" />
</ItemGroup>
```

## Troubleshooting

### "Program does not contain a static 'Main' method"
Prova generates the entry point for you. If you see this error, ensure that:
1. You do not have a `Program.cs` with a `Main` method.
2. If you do use top-level statements, ensure you are not conflicting with Prova's entry point generator.

### AOT Warnings
You may see warnings related to reflection-heavy libraries (like `Newtonsoft.Json`). Prova is fully AOT-compatible, but third-party libraries you use in your tests must also be trim-compatible or have appropriate suppression.

> [!NOTE]
> Prova's assertion library is designed to be fully AOT-safe.

## Next Steps

Once installed, proceed to **[Writing Tests](./writing-tests.md)** to structure your first test suite.
