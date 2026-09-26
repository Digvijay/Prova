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
        /// <typeparam name="T">The type of the values being compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <remarks>
        /// Collections are compared element by element. The default equality comparer compares
        /// arrays and lists by reference, so <c>Assert.Equal(new[] { 1, 2 }, new[] { 1, 2 })</c>
        /// would otherwise fail — an assertion that fails on equal input is worse than no
        /// assertion at all, because it teaches people to distrust the failure.
        /// </remarks>
        public static void Equal<T>(T expected, T actual)
        {
            if (!AreEqual(expected, actual))
            {
                throw new AssertException($"Assert.Equal() Failure\nExpected: {Describe(expected)}\nActual:   {Describe(actual)}");
            }
        }

        /// <summary>Verifies that two objects are not equal.</summary>
        /// <typeparam name="T">The type of the values being compared.</typeparam>
        /// <param name="expected">The value the actual value must differ from.</param>
        /// <param name="actual">The actual value.</param>
        /// <remarks>
        /// Uses the same collection-aware comparison as <see cref="Equal{T}"/>, so the two are
        /// exact opposites rather than two subtly different notions of equality.
        /// </remarks>
        public static void NotEqual<T>(T expected, T actual)
        {
            if (AreEqual(expected, actual))
            {
                throw new AssertException($"Assert.NotEqual() Failure\nExpected: not {Describe(expected)}\nActual:       {Describe(actual)}");
            }
        }

        private static bool AreEqual<T>(T expected, T actual)
        {
            // Strings are enumerable, but comparing them character by character would only lose
            // the far better message the default comparer already produces.
            if (expected is not string
                && expected is System.Collections.IEnumerable expectedSequence
                && actual is System.Collections.IEnumerable actualSequence)
            {
                return SequencesEqual(expectedSequence, actualSequence);
            }

            return System.Collections.Generic.EqualityComparer<T>.Default.Equals(expected, actual);
        }

        private static bool SequencesEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual)
        {
            var expectedEnumerator = expected.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();

            try
            {
                while (true)
                {
                    var hasExpected = expectedEnumerator.MoveNext();
                    var hasActual = actualEnumerator.MoveNext();

                    if (hasExpected != hasActual)
                    {
                        return false;
                    }

                    if (!hasExpected)
                    {
                        return true;
                    }

                    // Recurse so that nested collections compare by value too.
                    if (!AreEqual<object?>(expectedEnumerator.Current, actualEnumerator.Current))
                    {
                        return false;
                    }
                }
            }
            finally
            {
                (expectedEnumerator as IDisposable)?.Dispose();
                (actualEnumerator as IDisposable)?.Dispose();
            }
        }

        private static string Describe(object? value)
        {
            if (value is null)
            {
                return "null";
            }

            if (value is string text)
            {
                return text;
            }

            if (value is System.Collections.IEnumerable sequence)
            {
                var items = new System.Collections.Generic.List<string>();
                foreach (var item in sequence)
                {
                    items.Add(Describe(item));

                    // A failure message is for reading, not for dumping the whole collection.
                    if (items.Count == 10)
                    {
                        items.Add("...");
                        break;
                    }
                }

                return "[" + string.Join(", ", items) + "]";
            }

            return value.ToString() ?? "null";
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
        public static void NotNull(object? item, string? userMessage = null)
        {
            if (item is null)
            {
                throw new AssertException(userMessage ?? "Assert.NotNull() Failure");
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

        /// <summary>Verifies that a string starts with a given substring.</summary>
        public static void StartsWith(string expectedStart, string? actualString)
        {
            if (actualString == null || !actualString.StartsWith(expectedStart, StringComparison.Ordinal))
            {
                throw new AssertException($"Assert.StartsWith() Failure\nExpected to start with: {expectedStart}\nActual: {actualString ?? "(null)"}");
            }
        }
    }
}
