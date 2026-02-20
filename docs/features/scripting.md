# Scripting Support (Single-File Test Suites)

Prova supports a lightweight, scripting-like experience for writing tests using C# Top-Level Statements. This allows you to define tests and your execution logic in a single file, perfect for rapid prototyping, learning, or simple test utilities.

## The Concept

Instead of a complex project structure, you can have a single `Program.cs` file that contains both your test entry point and your test classes. You can run this file directly using `dotnet run`.

## Getting Started

### 1. Create a minimal project file

Create a `Tests.csproj` (or any name) with references to Prova:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <!-- Essential for AOT Source Generation -->
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\src\Prova.Core\Prova.Core.csproj" />
    <ProjectReference Include="..\src\Prova.Generators\Prova.Generators.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
  </ItemGroup>
</Project>
```

> **Note**: If using the NuGet package, simply replace the `ProjectReference`s with `<PackageReference Include="Prova" Version="..." />`.

### 2. Write your tests script

Create a `Program.cs` file using Top-Level Statements:

```csharp
using Prova;
using System;
using System.Threading.Tasks;

Console.WriteLine("🚀 Running My Scripted Tests...");

// run the tests!
await Prova.TestRunnerExecutor.RunAllAsync(args);

Console.WriteLine("✅ Done.");

// Define your tests in the same file
namespace MyScript
{
    public class QuickTests
    {
        [Fact]
        public void MathWorks()
        {
            Assert.Equal(4, 2 + 2);
        }

        [Fact]
        public async Task NetworkCheck()
        {
            await Task.Delay(100);
            Assert.True(true);
        }
    }
}
```

### 3. Run it!

```bash
dotnet run
```

Or, to pass arguments:

```bash
dotnet run -- --list-tests
```

## How It Works

Prova's Source Generator is compatible with Top-Level Statements. It scans all classes in the compilation, even those defined in the same file as the top-level entry point. The generated `TestRunnerExecutor` is fully accessible from your top-level code, giving you full control over when and how tests are run.

## Benefits

- **Zero Boilerplate**: No need for a separate `Startup` class or complex `Main` method.
- **Immediate Feedback**: Edit one file, run, and see results.
- **Full Power**: You still get full DI, Lifecycle Hooks, and Parallel execution support.
