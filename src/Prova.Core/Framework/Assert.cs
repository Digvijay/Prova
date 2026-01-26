using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Prova
{
    /// <summary>
    /// Contains various static methods that are used to verify conditions in unit tests.
    /// </summary>
    public static partial class Assert
    {
        /// <summary>Verifies that a condition is true.</summary>
        public static void True([DoesNotReturnIf(false)] bool condition, string? userMessage = null)
        {
            if (!condition)
            {
                throw new AssertException(userMessage ?? "Assert.True() Failure");
            }
        }

        /// <summary>Verifies that a string contains a given substring.</summary>
        public static void Contains(string expectedSubstring, string? actualString)
        {
            if (actualString == null || !actualString.Contains(expectedSubstring))
            {
                throw new AssertException($"Assert.Contains() Failure\nExpected to contain: {expectedSubstring}\nActual: {actualString ?? "(null)"}");
            }
        }

        /// <summary>Verifies that a string does not contain a given substring.</summary>
        public static void DoesNotContain(string expectedSubstring, string? actualString)
        {
            if (actualString != null && actualString.Contains(expectedSubstring))
            {
                throw new AssertException($"Assert.DoesNotContain() Failure\nExpected NOT to contain: {expectedSubstring}\nActual: {actualString}");
            }
        }

        /// <summary>Verifies that a collection contains a given item.</summary>
        public static void Contains<T>(T expected, System.Collections.Generic.IEnumerable<T> collection)
        {
             if (!System.Linq.Enumerable.Contains(collection, expected))
             {
                 throw new AssertException($"Assert.Contains() Failure\nCollection did not contain expected item: {expected}");
             }
        }
        
        /// <summary>Verifies that a collection does not contain a given item.</summary>
        public static void DoesNotContain<T>(T expected, System.Collections.Generic.IEnumerable<T> collection)
        {
             if (System.Linq.Enumerable.Contains(collection, expected))
             {
                 throw new AssertException($"Assert.DoesNotContain() Failure\nCollection contained unexpected item: {expected}");
             }
        }

        /// <summary>Verifies that a collection is empty.</summary>
        public static void Empty(System.Collections.IEnumerable collection)
        {
            var enumerator = collection.GetEnumerator();
            if (enumerator.MoveNext())
            {
                throw new AssertException("Assert.Empty() Failure\nCollection was not empty");
            }
        }

        /// <summary>Verifies that a collection is not empty.</summary>
        public static void NotEmpty(System.Collections.IEnumerable collection)
        {
            var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new AssertException("Assert.NotEmpty() Failure\nCollection was empty");
            }
        }
        
        /// <summary>Verifies that a collection contains exactly one item.</summary>
#pragma warning disable CA1720
        public static void Single(System.Collections.IEnumerable collection)
#pragma warning restore CA1720
        {
            var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext()) throw new AssertException("Assert.Single() Failure\nCollection was empty");
            if (enumerator.MoveNext()) throw new AssertException("Assert.Single() Failure\nCollection had more than one element");
        }

        /// <summary>Verifies that an object is of the given type.</summary>
        public static void IsType<T>(object? item)
        {
            IsType(typeof(T), item);
        }

        /// <summary>Verifies that an object is of the given type.</summary>
        public static void IsType(Type expectedType, object? item)
        {
            if (item == null || item.GetType() != expectedType)
            {
                throw new AssertException($"Assert.IsType() Failure\nExpected: {expectedType.Name}\nActual:   {item?.GetType().Name ?? "(null)"}");
            }
        }
        
        /// <summary>Verifies that an object is not of the given type.</summary>
        public static void IsNotType<T>(object? item)
        {
            IsNotType(typeof(T), item);
        }

        /// <summary>Verifies that an object is not of the given type.</summary>
        public static void IsNotType(Type expectedType, object? item)
        {
            if (item != null && item.GetType() == expectedType)
            {
                throw new AssertException($"Assert.IsNotType() Failure\nExpected any type but: {expectedType.Name}\nActual:                {expectedType.Name}");
            }
        }

        /// <summary>Verifies that two objects are the same instance.</summary>
        public static void Same(object? expected, object? actual)
        {
            if (!object.ReferenceEquals(expected, actual))
            {
                throw new AssertException($"Assert.Same() Failure\nExpected same instance");
            }
        }

        /// <summary>Verifies that two objects are not the same instance.</summary>
        public static void NotSame(object? expected, object? actual)
        {
            if (object.ReferenceEquals(expected, actual))
            {
                throw new AssertException($"Assert.NotSame() Failure\nExpected different instances");
            }
        }

        /// <summary>Verifies that a condition is false.</summary>
        public static void False([DoesNotReturnIf(true)] bool condition, string? userMessage = null)
        {
            if (condition)
            {
                throw new AssertException(userMessage ?? "Assert.False() Failure");
            }
        }

        /// <summary>Verifies that two objects are equal.</summary>
        public static void Equal<T>(T expected, T actual)
        {
            if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new AssertException($"Assert.Equal() Failure\nExpected: {expected}\nActual:   {actual}");
            }
        }
        
        /// <summary>Verifies that an object is null.</summary>
        public static void Null(object? item)
        {
            if (item is not null)
            {
                throw new AssertException($"Assert.Null() Failure\nExpected: null\nActual:   {item}");
            }
        }

        /// <summary>Verifies that an object is not null.</summary>
        public static void NotNull(object? item)
        {
            if (item is null)
            {
                throw new AssertException("Assert.NotNull() Failure");
            }
        }

        /// <summary>Fails the test with the given message.</summary>
        public static void Fail(string message)
        {
            throw new AssertException(message);
        }

        /// <summary>Verifies that the given code throws an exception of the given type.</summary>
        public static T Throws<T>(Action testCode) where T : Exception
        {
            try
            {
                testCode();
            }
            catch (T exception)
            {
                return exception;
            }
            catch (Exception ex)
            {
                throw new AssertException($"Assert.Throws() Failure\nExpected: {typeof(T).Name}\nActual:   {ex.GetType().Name}");
            }

            throw new AssertException($"Assert.Throws() Failure\nExpected: {typeof(T).Name}\nActual:   No exception was thrown");
        }

        /// <summary>Verifies that the given async code throws an exception of the given type.</summary>
        public static async Task<T> ThrowsAsync<T>(Func<Task> testCode) where T : Exception
        {
            try
            {
                await testCode();
            }
            catch (T exception)
            {
                return exception;
            }
            catch (Exception ex)
            {
                throw new AssertException($"Assert.ThrowsAsync() Failure\nExpected: {typeof(T).Name}\nActual:   {ex.GetType().Name}");
            }

            throw new AssertException($"Assert.ThrowsAsync() Failure\nExpected: {typeof(T).Name}\nActual:   No exception was thrown");
        }
    }
}
