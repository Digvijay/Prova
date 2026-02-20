# Executive Summary: Prova
*Reference Implementation for Native AOT Testing*

> **Project Scope**: Prova is a standalone **reference implementation** and an **experimental research project**. It showcases the potential of Roslyn-based testing architectures. It is a research initiative and is not associated with Microsoft Corporation.

## Project Objectives
**Prova** demonstrates the capabilities of the modern .NET ecosystem, specifically leveraging **Roslyn Source Generators** to shift test discovery and orchestration from **Runtime Reflection** to **Compile Time**.

## Key Architectural Principles

### 1. Compile-Time Generation
Legacy frameworks rely on runtime reflection for discovery, which incurs startup costs and limits optimization. Prova generates the test runner harnessing code during compilation.
- **Legacy**: Runtime Assembly Scanning -> Type Reflection -> Dynamic Invocation.
- **Prova**: Source Generation -> Static Linking -> Direct Execution.

### 2. Enhanced Developer Experience
Leveraging compiler access allows for features that are difficult to implement in runtime-only frameworks:
- **Documentation Integration**: XML documentation comments (`/// <summary>`) are extracted at compile-time and included in test reports.
- **Selective Compilation**: The `[Focus]` attribute instructs the generator to emit code only for the target test, significantly reducing the debug loop.

### 3. Native AOT Compatibility
Prova is designed for **Native AOT**. Test suites can be compiled into standalone, optimized binaries that operate without a full .NET runtime dependency, ideal for containerized or constrained environments.

## Integration
Prova serves as a reference for integrating with other AOT-compatible tools, such as **Skugga** (AOT Mocks). It demonstrates patterns for compile-time heuristic detection of dependencies to reduce boilerplate without runtime coupling.

## Conclusion
Prova provides a blueprint for high-performance, AOT-safe testing infrastructures. It serves as a proof-of-concept for framework authors looking to modernize their architecture using strict static analysis and source generation.
