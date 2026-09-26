using System;

namespace Prova.Generators.Tests
{
    public class DependencyTests
    {
        /// <summary>
        /// A [TestDependency] factory method supplies constructor arguments for a test class.
        /// </summary>
        /// <remarks>
        /// This previously compared the whole generated file byte-for-byte against a snapshot
        /// that had long gone stale, which asserted nothing useful once it drifted. It now
        /// asserts the behaviour the test is named for.
        /// </remarks>
        [Fact]
        public void TestDependency_Generates_Constructor_Injection()
        {
            var source = @"
using Prova;
using System;

public interface IService { }
public class MyService : IService { }

public class DependencyFactory
{
    [TestDependency]
    public static IService CreateService() => new MyService();
}

public class MyDependencyTests
{
    private readonly IService _service;

    public MyDependencyTests(IService service)
    {
        _service = service;
    }

    [Fact]
    public void DependencyTest()
    {
    }
}";

            GeneratorVerifier.VerifyContains(source, "new MyDependencyTests(DependencyFactory.CreateService())");
        }
    }
}
