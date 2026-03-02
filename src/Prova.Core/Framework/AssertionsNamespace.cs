namespace Prova.Assertions
{
    /// <summary>
    /// Assertion methods for Prova tests.
    /// This namespace re-exports <see cref="Prova.Assert"/> so that
    /// <c>using Prova.Assertions;</c> works as documented.
    /// </summary>
    public static class Assert
    {
        /// <summary>Verifies that a condition is true.</summary>
        public static void True(bool condition, string? userMessage = null) => Prova.Assert.True(condition, userMessage);

        /// <summary>Verifies that a condition is false.</summary>
        public static void False(bool condition, string? userMessage = null) => Prova.Assert.False(condition, userMessage);

        /// <summary>Verifies that two objects are equal.</summary>
        public static void Equal<T>(T expected, T actual) => Prova.Assert.Equal(expected, actual);

        /// <summary>Verifies that an object is null.</summary>
        public static void Null(object? item) => Prova.Assert.Null(item);

        /// <summary>Verifies that an object is not null.</summary>
        public static void NotNull(object? item, string? userMessage = null) => Prova.Assert.NotNull(item, userMessage);

        /// <summary>Verifies that a string contains a given substring.</summary>
        public static void Contains(string expectedSubstring, string? actualString) => Prova.Assert.Contains(expectedSubstring, actualString);

        /// <summary>Verifies that a string does not contain a given substring.</summary>
        public static void DoesNotContain(string expectedSubstring, string? actualString) => Prova.Assert.DoesNotContain(expectedSubstring, actualString);

        /// <summary>Verifies that a collection contains a given item.</summary>
        public static void Contains<T>(T expected, System.Collections.Generic.IEnumerable<T> collection) => Prova.Assert.Contains(expected, collection);

        /// <summary>Verifies that a collection does not contain a given item.</summary>
        public static void DoesNotContain<T>(T expected, System.Collections.Generic.IEnumerable<T> collection) => Prova.Assert.DoesNotContain(expected, collection);

        /// <summary>Verifies that a collection is empty.</summary>
        public static void Empty(System.Collections.IEnumerable collection) => Prova.Assert.Empty(collection);

        /// <summary>Verifies that a collection is not empty.</summary>
        public static void NotEmpty(System.Collections.IEnumerable collection) => Prova.Assert.NotEmpty(collection);

        /// <summary>Verifies that a collection contains exactly one item.</summary>
#pragma warning disable CA1720
        public static void Single(System.Collections.IEnumerable collection) => Prova.Assert.Single(collection);
#pragma warning restore CA1720

        /// <summary>Verifies that an object is of the given type.</summary>
        public static void IsType<T>(object? item) => Prova.Assert.IsType<T>(item);

        /// <summary>Verifies that an object is of the given type.</summary>
        public static void IsType(System.Type expectedType, object? item) => Prova.Assert.IsType(expectedType, item);

        /// <summary>Verifies that an object is not of the given type.</summary>
        public static void IsNotType<T>(object? item) => Prova.Assert.IsNotType<T>(item);

        /// <summary>Verifies that an object is not of the given type.</summary>
        public static void IsNotType(System.Type expectedType, object? item) => Prova.Assert.IsNotType(expectedType, item);

        /// <summary>Verifies that two objects are the same instance.</summary>
        public static void Same(object? expected, object? actual) => Prova.Assert.Same(expected, actual);

        /// <summary>Verifies that two objects are not the same instance.</summary>
        public static void NotSame(object? expected, object? actual) => Prova.Assert.NotSame(expected, actual);

        /// <summary>Fails the test with the given message.</summary>
        public static void Fail(string message) => Prova.Assert.Fail(message);

        /// <summary>Verifies that the given code throws an exception of the given type.</summary>
        public static T Throws<T>(System.Action testCode) where T : System.Exception => Prova.Assert.Throws<T>(testCode);

        /// <summary>Verifies that the given async code throws an exception of the given type.</summary>
        public static System.Threading.Tasks.Task<T> ThrowsAsync<T>(System.Func<System.Threading.Tasks.Task> testCode) where T : System.Exception => Prova.Assert.ThrowsAsync<T>(testCode);

        /// <summary>Verifies that a string starts with a given substring.</summary>
        public static void StartsWith(string expectedStart, string? actualString) => Prova.Assert.StartsWith(expectedStart, actualString);
    }
}
