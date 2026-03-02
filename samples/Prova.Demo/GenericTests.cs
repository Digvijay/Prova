using Prova;

namespace Prova.Demo;

// Demonstrate Generic Test Class
[Variant(typeof(int))]
[Variant(typeof(string))]
public class GenericStorageTests<T>
{
    [Fact]
    public void Storage_Can_Handle_Type()
    {
        // In a real test, we would do more, but here we just prove it runs
        Console.WriteLine($"Running storage test for type: {typeof(T).Name}");
    }
}

// Demonstrate Generic Test Method
public class AlgorithmTests
{
    [Variant(typeof(int))]
    [Variant(typeof(double))]
    [Fact]
    public void Sorting_Works_For_Numeric_Types<T>()
    {
        Console.WriteLine($"Running sorting test for type: {typeof(T).Name}");
    }

    [Variant(typeof(int))]
    [Theory]
    [InlineData(42)]
    [InlineData(100)]
    public void Mixed_Generic_And_Theory<T>(T value)
    {
        Console.WriteLine($"Running mixed test for type: {typeof(T).Name} with value: {value}");
    }
}

public class MultiVariantTests
{
    [Variant(typeof(int), typeof(string))]
    [Variant(typeof(string), typeof(int))]
    [Fact]
    public void BiDirectional_Mapping<TKey, TValue>()
    {
        Console.WriteLine($"Testing mapping between {typeof(TKey).Name} and {typeof(TValue).Name}");
    }
}
