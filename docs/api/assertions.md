# Assertions Reference ✅

Prova includes a standard suite of assertions for validating test results.

## Equality
- `Assert.Equal(expected, actual)`
- `Assert.NotEqual(expected, actual)`
- `Assert.StrictEqual(expected, actual)`
- `Assert.NotStrictEqual(expected, actual)`

## Boolean & Nullability
- `Assert.True(condition)`
- `Assert.False(condition)`
- `Assert.Null(obj)`
- `Assert.NotNull(obj)`

## Reference Equality
- `Assert.Same(expected, actual)`
- `Assert.NotSame(expected, actual)`

## Collections
- `Assert.Empty(collection)`
- `Assert.NotEmpty(collection)`
- `Assert.Single(collection)`
- `Assert.Contains(item, collection)`
- `Assert.DoesNotContain(item, collection)`
- `Assert.All(collection, action)`

## Strings
- `Assert.Contains(substring, string)`
- `Assert.DoesNotContain(substring, string)`
- `Assert.StartsWith(expected, string)`
- `Assert.EndsWith(expected, string)`
- `Assert.Matches(regex, string)`

## Exceptions
- `Assert.Throws<T>(action)`
- `Assert.ThrowsAsync<T>(asyncAction)`
- `Assert.ThrowsAny<T>(action)`
- `Assert.ThrowsAnyAsync<T>(asyncAction)`
