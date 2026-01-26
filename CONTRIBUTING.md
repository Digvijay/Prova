# Contributing to Prova

> [!NOTE]
> Prova is an **Experimental Research Project** and community-driven initiative. It is a standalone reference implementation for Native AOT testing and is not an official Microsoft product.

## Getting Started

1.  **Fork and Clone**: Fork the repo and clone it locally.
2.  **Environment**: You need `dotnet sdk 8.0` or higher.
3.  **Build**:
    ```bash
    dotnet build
    ```
4.  **Run Tests**:
    ```bash
    dotnet run --project tests/Prova.Core.Tests/Prova.Core.Tests.csproj
    ```

## Development Workflow

- **Branching**: Create a feature branch for your changes (e.g., `feature/awesome-new-assert`).
- **Committing**: Keep commits atomic and descriptive.
- **Testing**: **ALWAYS** add a test case in `Prova.Core.Tests` for your new feature.

## Project Structure

- `src/Prova.Core`: The runtime library (Attributes, Assert, Reporters).
- `src/Prova.Generators`: The magic 🧙‍♂️. Roslyn Source Generators that build the runner.
- `tests/Prova.Core.Tests`: The test suite (yes, Prova tests itself!).

## Submitting a Pull Request

1.  Push your branch to your fork.
2.  Open a Pull Request against `main`.
3.  Ensure CI passes.
4.  Wait for review! We try to review PRs within 48 hours.

## Code of Conduct

Be kind, respectful, and professional. We are all here to learn and build cool stuff.
