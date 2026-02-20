# Contributing to Prova

> [!NOTE]
> Prova is an **open-source, community-driven** testing framework for .NET. Contributions of all sizes are welcome!

## Getting Started

1.  **Fork and Clone**: Fork the repo and clone it locally.
2.  **Environment**: You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).
3.  **Build**:
    ```bash
    dotnet build
    ```
4.  **Run Tests**:
    ```bash
    dotnet test tests/Prova.Core.Tests
    dotnet test tests/Prova.Generators.Tests
    ```

## Development Workflow

- **Branching**: Create a feature branch for your changes (e.g., `feature/awesome-new-assert`).
- **Committing**: Keep commits atomic and descriptive.
- **Testing**: **ALWAYS** add a test case in `Prova.Core.Tests` or `Prova.Generators.Tests` for your new feature.

## Project Structure

| Directory | Purpose |
|-----------|---------|
| `src/Prova.Core` | The runtime library (Attributes, Assert, Reporters) |
| `src/Prova.Generators` | Roslyn Source Generators that build the runner |
| `src/Prova.Analyzers` | Migration analyzers and code fixers |
| `src/Prova.AspNetCore` | ASP.NET Core integration package |
| `src/Prova.Playwright` | Playwright browser testing package |
| `src/Prova.Testcontainers` | Testcontainers integration package |
| `src/Prova.FsCheck` | FsCheck property-based testing package |
| `tests/` | Test suite (Prova tests itself!) |
| `samples/` | Sample projects demonstrating features |
| `docs/` | VitePress documentation site |

## Running the Documentation Site Locally

```bash
cd docs
npm install
npm run docs:dev
```

## Submitting a Pull Request

1.  Push your branch to your fork.
2.  Open a Pull Request against `master`.
3.  Ensure CI passes (build + tests).
4.  Wait for review! We try to review PRs within 48 hours.

## Code of Conduct

Be kind, respectful, and professional. We are all here to learn and build cool stuff.
