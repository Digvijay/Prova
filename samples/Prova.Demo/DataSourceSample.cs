using System.Collections.Generic;
using Prova;
using Prova.Core; // Ensure we use Prova.Core namespace

namespace Prova.Demo
{
    // A service that provides data
    public class UserDataProvider
    {
        public IEnumerable<object[]> GetUsers()
        {
            yield return new object[] { 1, "Alice" };
            yield return new object[] { 2, "Bob" };
        }
    }

    // A class data source
    public class OrderDataSource : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "Order_101", 99.99m };
            yield return new object[] { "Order_102", 45.50m };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class DataSourceSample
    {
        // Setup DI for the data provider

        [Theory]
        [MethodDataSource(nameof(UserDataProvider.GetUsers), MemberType = typeof(UserDataProvider))]
        public void Test_MethodDataSource(int id, string name)
        {
            Assert.True(id > 0);
            Assert.NotNull(name);
        }

        [Theory]
        [ClassDataSource(typeof(OrderDataSource))]
        public void Test_ClassDataSource(string orderId, decimal amount)
        {
            Assert.StartsWith("Order_", orderId);
            Assert.True(amount > 0);
        }
    }

    // Class-level data source provides parameters to the constructor
    [ClassDataSource(typeof(OrderDataSource))]
    public class ClassLevelDataSourceSample
    {
        private readonly string _orderId;
        private readonly decimal _amount;

        public ClassLevelDataSourceSample(string orderId, decimal amount)
        {
            _orderId = orderId;
            _amount = amount;
        }

        [Fact]
        public void Test_Constructor_Injection()
        {
            Assert.StartsWith("Order_", _orderId);
            Assert.True(_amount > 0);
        }
    }
}
