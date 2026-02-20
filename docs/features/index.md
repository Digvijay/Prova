# Advanced Features

Prova comes with a suite of advanced testing features designed for modern development workflows.

## 📊 Integrated Code Coverage
Native LCOV support without external tools. See [Code Coverage](./coverage) for details.

```bash
dotnet run -- --coverage
```

## 🔁 Flow Control
Fine-tune execution with Retry, Repeat, and per-test Culture settings. See [Flow Control](./flow-control) for details.

```csharp
[Fact, Retry(3)] // Handle flakiness
public void FlakyTest() { ... }

[Fact, Repeat(100)] // Stress testing
public void HotPath() { ... }

[Fact, Culture("fr-FR")] // Localization
public void FrenchTest() { ... }
```

## ⏱️ Timeout
Enforce maximum execution time for tests. See [Timeouts](./timeouts) for details.

```csharp
[Fact, Timeout(1000)] // 1 second limit
public async Task LongRunningTest()
{
    await Task.Delay(500);
}
```

## 🚦 Resource Constraints
Fine-grained parallelism control. See [Resource Constraints](./resource-constraints) for details.

```csharp
[Fact, NotInParallel("Database")]
public async Task WriteToDB() { ... }
```

## 🔍 Focus
Run only specific tests during development.

```csharp
[Fact, Focus]
public void WipTest()
{
    // Run only this test with `dotnet test`
}
```

## 💾 Allocation Governance
Enforce memory allocation limits on your tests to prevent regressions in hot paths.

```csharp
[Fact, MaxAlloc(1024)] // 1KB limit
public void HotPathTest()
{
    // Fails if allocates > 1024 bytes
}
```

## 🛠️ Data Generators
Custom, reusable data sources. See [Data Generators](./data-generators) for details.

```csharp
public class MyDataAttribute : DataSourceGeneratorAttribute
{
    public override IEnumerable<object?[]> GetData()
    {
        yield return new object?[] { 1, "A" };
    }
}
```

## 💉 Dependency Injection
Zero-reflection DI for AOT compatibility. See [Dependency Injection](../concepts/di) for details.

```csharp
[ConfigureServices]
public static void Configure(ProvaServiceProvider services)
{
    services.AddSingleton<IService>(() => new Service());
}
```

## 💉 DI Data Sources
Source test data from services registered in the DI container. See [DI Data Sources](./di-data-sources) for details.

```csharp
[Theory]
[DependencyInjectionDataSource(typeof(MyDataProvider), "GetData")]
public void Test(int value) { ... }
```
## 🔄 Lifecycle Hooks
Granular control with Test, Class, and Assembly-level setup/teardown. See [Lifecycle Hooks](./lifecycle-hooks) for more info.

```csharp
[Before]
public void Setup() { ... }

[Before(HookScope.Class)]
public static void GlobalSetup() { ... }
```

## 🏭 Class Factories
Control test class instantiation for custom DI or complex logic. See [Class Factories](./class-factories) for more info.

```csharp
[ClassFactory(typeof(MyFactory))]
public class MyTests { ... }
```

## 🔍 Debugging Support
Automatic crash and hang dump generation. See [Debugging](./debugging) for details.

```bash
# Capture a dump if process crashes
dotnet test -- --crashdump

# Capture a dump if tests hang
dotnet test -- --hangdump --hangdump-timeout 5m
```

## 🌐 ASP.NET Core Integration
Seamless integration with `WebApplicationFactory` for AOT-compatible end-to-end tests. See [ASP.NET Core](../integrations/aspnet-core) for details.

```csharp
[ConfigureServices]
public static void Configure(ProvaServiceProvider services)
{
    // Register WebApplicationFactory
    services.AddSingleton<WebApplicationFactory<Program>>(() => new ProvaWebApplicationFactory<Program>());
}
```
