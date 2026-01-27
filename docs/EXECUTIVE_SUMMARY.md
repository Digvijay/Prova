# Executive Summary: Prova 🇸🇪
*A Standalone Reference Implementation for Native AOT Testing*

> [!CAUTION]
> **Independent Project Notice**: Prova is a community-driven **Experimental Research Project**. It is a standalone reference implementation for AOT-native testing. While independent of Microsoft, it provides **Hybrid MTP Compatibility** for modern ecosystem integration.

## The Mission
**Prova** (Swedish: *Test*) is built to demonstrate the extreme capabilities of the modern .NET ecosystem, specifically the power of **Roslyn Source Generators**. It is a purely independent research initiative.

It is designed to serve as an inspiration for the ecosystem, showcasing how we can achieve **Zero-Overhead** and **Native AOT** compatibility by moving discovery from **Runtime Reflection** to **Compile Time**.

## The Architecture: "Compile-Time is the new Runtime"
Legacy frameworks were designed in the "Reflection Era." Prova reimagines testing for the "AOT Era."

### 1. ⚡ Zero-Overhead Performance
By generating the test runner code at compile time, Prova starts instantly. 
- **Legacy**: Scan assembly -> Reflection -> Loading.
- **Prova**: `Main()` -> `await TestRunner.RunAllAsync()`.

### 2. 🧙‍♂️ "Magical" Developer Experience
We leveraged the compiler to build features impossible in runtime-only frameworks:
- **Magic Docs**: XML documentation comments (`/// <summary>`) are extracted at compile-time and displayed in the test output.
- **Compiler-Enforced Focus**: `[Focus]` works by telling the generator to *only emit code for that test*. 

### 3. 🛡️ Native AOT Ready
Prova is **100% Native AOT compatible**. You can compile your test suite into a standalone, optimized binary that runs anywhere without the .NET runtime installed.

## The Nordic Suite 🛡️
Prova is the "Voice" of the Nordic AOT Suite. It is designed to integrate seamlessly with ecosystem tools like **Skugga** (AOT Mocks).
- **Smart Verify**: Prova automatically detects Skugga mocks in your test class and ensures verification happens, eliminating boilerplate.

## Conclusion
Prova is not just a framework; it's a blueprint for the future of .NET testing. By embracing modern compiler capabilities, we unlock a "Developer Joy" feature set that was previously impossible.

**Ready for the future of .NET testing?**
