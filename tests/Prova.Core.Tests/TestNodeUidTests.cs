using System.Linq;
using System.Threading.Tasks;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Pins the identity the adapter reports for each test.
    /// </summary>
    /// <remarks>
    /// The identifier used to be derived from the display name. A <c>[DisplayName]</c> containing
    /// no format placeholders produces the same string for every row of a theory, so every row was
    /// reported under one identifier and the rows collapsed into a single node: a failing row could
    /// be reported as passing because a later row overwrote it. Identity has to come from structure,
    /// not from a label a user is free to make ambiguous.
    /// </remarks>
    public class TestNodeUidTests
    {
        private static ProvaTest Row(string uniqueName, string displayName)
            => new ProvaTest
            {
                DisplayName = displayName,
                UniqueName = uniqueName,
                FullName = "Acme.OrderTests.Totals",
                ClassName = "Acme.OrderTests",
                ExecuteDelegate = () => Task.FromResult<string?>(null),
            };

        private static HybridMtpAdapter Adapter(params ProvaTest[] tests)
            => new HybridMtpAdapter(tests, new ProvaCapabilities());

        /// <summary>Rows labelled identically are still reported separately.</summary>
        [Fact]
        public void Theory_Rows_Sharing_A_DisplayName_Get_Distinct_Uids()
        {
            var first = Row("Acme.OrderTests.Totals(1, 2)", "Adds numbers");
            var second = Row("Acme.OrderTests.Totals(3, 4)", "Adds numbers");

            var adapter = Adapter(first, second);

            Assert.NotEqual(adapter.GetTestUid(first), adapter.GetTestUid(second));
        }

        /// <summary>An unambiguous structural name is used verbatim.</summary>
        [Fact]
        public void A_Uid_Is_The_Structural_Name_When_It_Is_Already_Unique()
        {
            var test = Row("Acme.OrderTests.Totals(1, 2)", "Adds numbers");

            var adapter = Adapter(test);

            Assert.Equal("Acme.OrderTests.Totals(1, 2)", adapter.GetTestUid(test));
        }

        /// <summary>Colliding identities are numbered in a stable order.</summary>
        [Fact]
        public void Genuinely_Identical_Identities_Are_Disambiguated_Deterministically()
        {
            // A hand-built ProvaTest, or a registration from an older generator, can still collide.
            // Rather than let two tests share one identifier, the duplicates are numbered.
            var first = Row("Acme.OrderTests.Totals", "Totals");
            var second = Row("Acme.OrderTests.Totals", "Totals");
            var third = Row("Acme.OrderTests.Totals", "Totals");

            var adapter = Adapter(first, second, third);

            Assert.Equal("Acme.OrderTests.Totals", adapter.GetTestUid(first));
            Assert.Equal("Acme.OrderTests.Totals#2", adapter.GetTestUid(second));
            Assert.Equal("Acme.OrderTests.Totals#3", adapter.GetTestUid(third));
        }

        /// <summary>No two tests in one run share an identifier.</summary>
        [Fact]
        public void Every_Test_In_A_Run_Has_A_Distinct_Uid()
        {
            var tests = Enumerable.Range(0, 20)
                .Select(_ => Row("Acme.OrderTests.Totals", "Adds numbers"))
                .ToArray();

            var adapter = Adapter(tests);

            var uids = tests.Select(t => adapter.GetTestUid(t)).ToArray();

            Assert.Equal(uids.Length, uids.Distinct().Count());
        }

        /// <summary>A test excluded by a filter has no identifier.</summary>
        [Fact]
        public void A_Test_The_Adapter_Does_Not_Hold_Has_No_Uid()
        {
            var held = Row("Acme.OrderTests.Totals(1, 2)", "Adds numbers");
            var stranger = Row("Acme.OrderTests.Totals(9, 9)", "Adds numbers");

            var adapter = Adapter(held);

            Assert.Null(adapter.GetTestUid(stranger));
        }
    }
}
