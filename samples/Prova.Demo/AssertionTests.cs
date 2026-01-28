using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prova;

namespace Prova.Demo
{
    /// <summary>Tests for verifying Assertion logic.</summary>
    public class AssertionTests
    {
        /// <summary>Verify collection assertions.</summary>
        [Fact]
        public void CollectionAssertions()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.Contains(2, list);
            Assert.DoesNotContain(4, list);
            Assert.NotEmpty(list);
            
            var empty = new List<int>();
            Assert.Empty(empty);
            
            var single = new List<string> { "Solo" };
            Assert.Single(single);
        }

        /// <summary>Verify string assertions.</summary>
        [Fact]
        public void StringAssertions()
        {
            Assert.Contains("World", "Hello World");
            Assert.DoesNotContain("Universe", "Hello World");
        }

        /// <summary>Verify type assertions.</summary>
        [Fact]
        public void TypeAssertions()
        {
            object str = "hello";
            Assert.IsType<string>(str);
            // Assert.IsType(typeof(string), str);
            Assert.IsNotType<int>(str);
        }

        /// <summary>Verify reference assertions.</summary>
        [Fact]
        public void ReferenceAssertions()
        {
            var obj1 = new object();
            var obj2 = obj1;
            var obj3 = new object();

            Assert.Same(obj1, obj2);
            Assert.NotSame(obj1, obj3);
            Assert.Null(null);
            Assert.NotNull(obj1);
        }

        /// <summary>Verify exception assertions.</summary>
        [Fact]
        public void ExceptionAssertions()
        {
            Assert.Throws<InvalidOperationException>(() => throw new InvalidOperationException("Boom"));
            
            var ex = Assert.Throws<InvalidOperationException>(() => throw new InvalidOperationException("Catch me"));
            Assert.Equal("Catch me", ex.Message);
        }

        /// <summary>Verify async exception assertions.</summary>
        [Fact]
        public async Task AsyncExceptionAssertions()
        {
            await Assert.ThrowsAsync<TaskCanceledException>(async () => 
            {
                await Task.Yield();
                throw new TaskCanceledException();
            });
        }
    }
}
