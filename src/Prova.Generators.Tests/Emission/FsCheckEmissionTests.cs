using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Prova.Generators.Tests.Emission
{
    public class FsCheckEmissionTests
    {
        [Fact]
        public Task Emit_FsCheck_Property()
        {
            var source = @"
using Prova.FsCheck;
using System;

namespace TestProject
{
    public class MyTests
    {
        [Property]
        public void MyProperty(int a, string b) { }
    }
}";
            return GeneratorVerifier.Verify(source);
        }

        [Fact]
        public Task Emit_FsCheck_Property_With_Config()
        {
            var source = @"
using Prova.FsCheck;
using System;

namespace TestProject
{
    public class MyTests
    {
        [Property(MaxTest = 500, Verbose = true)]
        public void MyProperty(int a) { }
    }
}";
            return GeneratorVerifier.Verify(source);
        }
    }
}
