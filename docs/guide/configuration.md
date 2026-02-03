# ⚙️ Configuration & Hierarchy

Prova supports a robust hierarchical configuration system that allows you to set defaults globally and override them locally.

## Precedence Rule
Settings are resolved in the following order (first match wins):
1. **Method Level** (`[Attribute]` on method)
2. **Class Level** (`[Attribute]` on class)
3. **Assembly Level** (`[assembly: Attribute]`)
4. **Global Default** (Framework Default)

---

## ⏱️ Timeout
Control how long a test runs before being aborted.

### Global Default
Set a safety net for all tests in the assembly.
```csharp
[assembly: Timeout(5000)] // 5 seconds
```

### Class Override
Enforce strict limits for performance-critical components.
```csharp
[Timeout(100)] // 100ms
public class ApiTests { ... }
```

### Method Override
Allow specific long-running tests.
```csharp
[Fact]
[Timeout(30000)] // 30s integration test
public async Task BigIntegrationTest() { ... }
```

---

## 🔁 Retry
Automatically retry flaky tests.

### Global Policy
Mitigate transient environment failures by retrying all failed tests once.
```csharp
[assembly: Retry(1)]
```

### Disable Retries
Sometimes you want zero tolerance for specific logic.
```csharp
[Retry(0)] // Disable retries for this class
public class TransactionTests { ... }
```
