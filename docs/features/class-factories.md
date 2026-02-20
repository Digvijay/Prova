# Class Factories

Prova allows you to control how test classes are instantiated by using custom class factories. This is particularly useful for complex setup scenarios, integration with custom Dependency Injection containers, or when you need to perform manual instantiation logic that goes beyond simple constructor injection.

## IClassConstructor\<T> Interface

To create a class factory, implement the `IClassConstructor<T>` interface where `T` is the type of your test class.

```csharp
public interface IClassConstructor<T> where T : class
{
    T CreateInstance(IServiceProvider services);
}
```

## Using [ClassFactory]

Apply the `[ClassFactory]` attribute to your test class to specify which factory should be used for its instantiation.

### Example

```csharp
using Prova;
using System;

// 1. Define your factory
public class MyTestFactory : IClassConstructor<MyTests>
{
    public MyTests CreateInstance(IServiceProvider services)
    {
        // Custom instantiation logic
        var dependency = services.GetService(typeof(IMyService)) as IMyService;
        return new MyTests(dependency, "initialized-via-factory");
    }
}

// 2. Apply [ClassFactory] to your test class
[ClassFactory(typeof(MyTestFactory))]
public class MyTests
{
    private readonly IMyService _service;
    private readonly string _tag;

    public MyTests(IMyService service, string tag)
    {
        _service = service;
        _tag = tag;
    }

    [Fact]
    public void Test_Something()
    {
        Assert.Equal("initialized-via-factory", _tag);
    }
}

// 3. Register your factory in ConfigureServices
public class Startup
{
    [ConfigureServices]
    public static void Configure(ProvaServiceProvider services)
    {
        services.AddTransient(() => new MyTestFactory());
    }
}
```

## Why use Class Factories?

- **Custom DI Integration**: If you are using a non-standard DI container, you can use a factory to resolve dependencies.
- **Manual Setup**: Perform complex initialization logic before the test class is even constructed.
- **Factory Pattern**: Use the factory pattern to manage the lifecycle of expensive test resources.

> [!TIP]
> Class factories are resolved from the test runner's internal service container. Make sure to register your factory using `[ConfigureServices]`.
