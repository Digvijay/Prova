using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    [InlineData("ClassRow1", 10)]
    [InlineData("ClassRow2", 20)]
    public class ConstructorDataTests
    {
        private readonly string _className;
        private readonly int _classValue;

        public ConstructorDataTests(string className, int classValue)
        {
            _className = className;
            _classValue = classValue;
        }

        [Fact]
        public void Class_Data_Is_Injected()
        {
            Assert.NotNull(_className);
            Assert.True(_classValue > 0);
        }

        [Theory]
        [InlineData("MethodRowA")]
        [InlineData("MethodRowB")]
        public void Combined_Theory(string methodName)
        {
            Assert.NotNull(methodName);
            // This test will run 4 times: (2 class rows) * (2 method rows)
        }
    }
}
