using System.Collections.Generic;
using FsCheck.Fluent;

namespace Prova.Generators.Tests
{
    /// <summary>
    /// Covers <see cref="Prova.FsCheck.FsCheckRunner"/>'s handling of the values forwarded from
    /// <c>[Property]</c>.
    /// </summary>
    /// <remarks>
    /// Regression tests. The runner previously called <c>Check.QuickThrowOnFailure</c> and threw
    /// the configuration dictionary away, so every documented knob on <c>[Property]</c>
    /// -- MaxTest, MaxFail, StartSize, EndSize, Verbose, QuietOnSuccess -- was silently ignored:
    /// <c>[Property(MaxTest = 1000)]</c> still ran exactly 100 cases. The emission tests could
    /// not catch this because they only assert the dictionary is populated, not that it is read.
    /// </remarks>
    public class FsCheckRunnerConfigTests
    {
        private static int RunCountingCases(Dictionary<string, string>? config)
        {
            var cases = 0;
            var property = Prop.ForAll<int>(_ =>
            {
                cases++;
                return true;
            });

            global::Prova.FsCheck.FsCheckRunner.Run(config, property);
            return cases;
        }

        [Fact]
        public void MaxTest_Is_Honoured()
        {
            Assert.Equal(7, RunCountingCases(new Dictionary<string, string> { ["MaxTest"] = "7" }));
        }

        [Fact]
        public void MaxTest_Above_The_Default_Is_Honoured()
        {
            // The defect this guards: 1000 used to run 100 cases.
            Assert.Equal(1000, RunCountingCases(new Dictionary<string, string> { ["MaxTest"] = "1000" }));
        }

        [Fact]
        public void Missing_Config_Falls_Back_To_The_FsCheck_Default()
        {
            Assert.Equal(100, RunCountingCases(null));
        }

        [Fact]
        public void Unparseable_Values_Do_Not_Throw_And_Keep_The_Default()
        {
            Assert.Equal(100, RunCountingCases(new Dictionary<string, string> { ["MaxTest"] = "not-a-number" }));
        }

        [Fact]
        public void Size_Bounds_Are_Honoured()
        {
            var sizes = new List<int>();
            var property = Prop.ForAll<int[]>(xs =>
            {
                sizes.Add(xs.Length);
                return true;
            });

            global::Prova.FsCheck.FsCheckRunner.Run(
                new Dictionary<string, string>
                {
                    ["MaxTest"] = "50",
                    ["StartSize"] = "3",
                    ["EndSize"] = "3",
                },
                property);

            // With both bounds pinned, FsCheck never generates beyond the requested size.
            Assert.True(sizes.Count > 0);
            Assert.True(sizes.TrueForAll(n => n <= 3));
        }
    }
}
