# Performance

Prova is designed for speed. Leveraging its AOT (Ahead-of-Time) compilation and parallel execution capabilities can significantly reduce your test suite's runtime.

## AOT Optimization

Unlike traditional test runners that rely on reflection at runtime, Prova generates execution code at compile-time.

- **Instant Startup**: Tests start executing immediately without a "discovery" phase or JIT overhead.
- **Small Footprint**: Prova results in smaller binaries, which is ideal for CI/CD environments and containerized builds.
- **Native AOT**: Prova is fully compatible with .NET's Native AOT publishing, allowing you to run your tests as standalone native binaries.

## Parallel Execution

Maximize your hardware by running tests in parallel.

### Enabling Parallelism
You can enable parallel execution at the assembly, class, or method level using the `[Parallel]` attribute.

```csharp
[assembly: Parallel(concurrency: 4)] // 4 tests at once
```

### Resource Constraints
Use `[NotInParallel]` or `[ParallelGroup]` to protect tests that share global state (like a database or file system).

```csharp
[NotInParallel] // Won't run with other tests marked the same
public void DatabaseTest() { ... }
```

## Lightweight Data

Data generation can become a bottleneck.

- **Prefer InlineData**: Use `[InlineData]` for simple parameter sets. It's the most efficient way to parameterize tests.
- **Lazy MemberData**: If using `[MemberData]`, ensure the data generation itself is efficient and doesn't perform heavy computations or I/O unless necessary.

## Filtering

Run only what you need. Prova's CLI allows for fast, hierarchical filtering so you don't waste time running irrelevant tests.

```bash
# Run only security-related tests
./MyTestProject --filter "Security.*"
```
