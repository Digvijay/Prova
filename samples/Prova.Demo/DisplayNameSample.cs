using Prova;
using System.Threading.Tasks;

namespace Prova.Demo
{
    public class DisplayNameSample
    {
        [Fact]
        [DisplayName("Static Test Name (No Args)")]
        public void StaticName()
        {
            Assert.True(true);
        }

        [Theory]
        [InlineData(1, "A")]
        [InlineData(2, "B")]
        [DisplayName("Value {0} corresponds to {1}")]
        public void DataDrivenDisplayName(int id, string label)
        {
            Assert.NotNull(label);
            Assert.True(id > 0);
        }

        [Fact]
        [DisplayName("Combinatorial: User {0} is Active: {1}")]
        public void CombinatorialDisplayName([Matrix("Alice", "Bob")] string user, [Matrix(true, false)] bool active)
        {
            Assert.NotNull(user);
        }
    }
}
