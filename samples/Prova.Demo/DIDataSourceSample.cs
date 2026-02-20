using System.Collections.Generic;
using Prova;
using Prova.Core;

namespace Prova.Demo
{
    public class DIDataSourceSample
    {
        [ConfigureServices]
        public static void Configure(ProvaServiceProvider services)
        {
            // Register the data provider and its dependencies
            services.AddSingleton<MyDIService>(() => new MyDIService());
            services.AddTransient<DataProvider>(() => new DataProvider(services.Get<MyDIService>()));
            services.AddSingleton<ClassDataProvider>(() => new ClassDataProvider());
        }

        [Theory]
        [DependencyInjectionDataSource(typeof(DataProvider), nameof(DataProvider.GetItems))]
        public void TestWithDIData(string item, int value)
        {
            Assert.NotNull(item);
            Assert.True(value >= 0);
        }

        [DependencyInjectionDataSource(typeof(ClassDataProvider))]
        public class ClassLevelDIDataTests
        {
            public ClassLevelDIDataTests(string item, int value)
            {
                Assert.NotNull(item);
            }

            [Fact]
            public void Test1()
            {
            }
        }
    }

    public class MyDIService
    {
        public string GetPrefix() => "Item_";
    }

    public class DataProvider
    {
        private readonly MyDIService _service;

        public DataProvider(MyDIService service)
        {
            _service = service;
        }

        public IEnumerable<object[]> GetItems()
        {
            yield return new object[] { _service.GetPrefix() + "1", 10 };
            yield return new object[] { _service.GetPrefix() + "2", 20 };
        }
    }

    public class ClassDataProvider : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "ClassData1", 1 };
            yield return new object[] { "ClassData2", 2 };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
