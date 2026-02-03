# Comparison Guide

Choosing a testing framework is a long-term commitment. Prova is built for the future of .NET, focusing heavily on **Native AOT**, **Zero Dependencies**, and **Modern C# Features**.

## Feature Matrix

| Feature | Prova | xUnit | NUnit | MSTest | TUnit |
| :--- | :---: | :---: | :---: | :---: | :---: |
| **Native AOT** | ✅ First-Class | ❌ | ❌ | ⚠️ Partial | ✅ |
| **Zero Dependencies** | ✅ | ❌ | ❌ | ❌ | ✅ |
| **Executable Tests** | ✅ | ❌ | ❌ | ⚠️ Runner | ✅ |
| **Parallel by Default** | ✅ | ✅ | ❌ | ❌ | ✅ |
| **Dependency Injection** | ✅ Native | ⚠️ Add-on | ❌ | ❌ | ✅ |
| **Global Hooks** | ✅ | ❌ | ✅ | ✅ | ✅ |

## vs xUnit
xUnit is the gold standard for .NET testing, known for its strict immutability and isolation.
- **Prova** adopts xUnit's "Constructor per Test" and `[Fact]` syntax, making migration easy.
- **Difference:** Prova is an executable, not a DLL. This means no reflection-based discovery at runtime, enabling Native AOT.

## vs NUnit / MSTest
Traditional frameworks heavily rely on mutable state (`[SetUp]`, `[TearDown]`) and static globals.
- **Prova** avoids mutable shared state by default but offers `IAsyncLifetime` and `[Before(Test)]` hooks when granular control is needed.

## vs TUnit
TUnit is a modern, AOT-focused framework that Prova draws significant inspiration from.
- **Similarities:** Both are AOT-first, executable-based, and heavily opinionated.
- **Difference:** Prova aims for a slightly more "Batteries Included" approach, bundling strict Assertion libraries (`Prova.Assertions`) and specialized Analyzers (`Prova.Analyzers`) by default, rather than relying on external packages.

## Why Choose Prova?

1.  **You need Native AOT**: If you are building AWS Lambda, Azure Functions, or CLI tools in Native AOT, Prova is the only viable choice alongside TUnit.
2.  **Performance**: Prova tests start instantly. Benchmarks show AOT test runners can be **up to 20x faster** than reflection-based runners (MSTest/xUnit/NUnit) in complex scenarios like Combinatorial Matrix tests.
3.  **Simplicity**: One package. No `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `coverlet`, etc. It just works.
