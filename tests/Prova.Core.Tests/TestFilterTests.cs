using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Covers name-based test selection and the identity that selection depends on.
    /// </summary>
    /// <remarks>
    /// Two defects meet in this file. The platform only offered <c>--filter-uid</c>, so running a
    /// single test meant first listing every test to find its UID; and the UID was derived from
    /// the display name, so a <c>[DisplayName]</c> with no format placeholders gave every data row
    /// of a theory the same identity and collapsed them into one node. A filter is only as good as
    /// the names it matches against, which is why both are tested together.
    /// </remarks>
    public class TestFilterTests
    {
        private static ProvaTest Test(string uniqueName, string fullName, string className, string displayName)
            => new ProvaTest
            {
                DisplayName = displayName,
                UniqueName = uniqueName,
                FullName = fullName,
                ClassName = className,
                ExecuteDelegate = () => Task.FromResult<string?>(null),
            };

        private static ProvaTest[] Sample() => new[]
        {
            Test("Acme.OrderTests.Totals(1)", "Acme.OrderTests.Totals", "Acme.OrderTests", "Adds numbers"),
            Test("Acme.OrderTests.Totals(2)", "Acme.OrderTests.Totals", "Acme.OrderTests", "Adds numbers"),
            Test("Acme.OrderTests.Discounts", "Acme.OrderTests.Discounts", "Acme.OrderTests", "Discounts"),
            Test("Acme.ShippingTests.Totals", "Acme.ShippingTests.Totals", "Acme.ShippingTests", "Shipping totals"),
        };

        private static string[] Names(IReadOnlyList<ProvaTest> tests)
            => tests.Select(t => t.UniqueName!).ToArray();

        /// <summary>With no filter supplied every test is returned and no arguments are consumed.</summary>
        [Fact]
        public void No_Filter_Returns_Every_Test()
        {
            var result = TestFilter.Apply(Sample(), System.Array.Empty<string>(), out var remaining);

            Assert.Equal(4, result.Count);
            Assert.Empty(remaining);
        }

        /// <summary>A pattern with no wildcard matches anywhere in the name.</summary>
        [Fact]
        public void Filter_Name_Without_Wildcard_Matches_A_Substring()
        {
            // A developer typing a bare method name expects it to find the test, not to have to
            // spell out the namespace and the data row.
            var args = new[] { "--filter-name", "Discounts" };
            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Single(result);
            Assert.Equal("Acme.OrderTests.Discounts", result[0].UniqueName);
        }

        /// <summary>A pattern containing a wildcard is anchored at both ends.</summary>
        [Fact]
        public void Filter_Name_With_Wildcard_Anchors_The_Whole_Name()
        {
            var args = new[] { "--filter-name=Acme.OrderTests.Totals*" };
            var expected = new[] { "Acme.OrderTests.Totals(1)", "Acme.OrderTests.Totals(2)" };

            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Equal(expected, Names(result));
        }

        /// <summary>The class filter selects every test declared by a class.</summary>
        [Fact]
        public void Filter_Class_Selects_A_Whole_Class()
        {
            var args = new[] { "--filter-class", "Acme.ShippingTests" };
            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Single(result);
            Assert.Equal("Acme.ShippingTests.Totals", result[0].UniqueName);
        }

        /// <summary>The method filter matches every data row of a theory.</summary>
        [Fact]
        public void Filter_Method_Ignores_The_Data_Row_Suffix()
        {
            // 'Totals' is one method name shared by two classes and three registrations.
            var args = new[] { "--filter-method", "Totals" };
            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Equal(3, result.Count);
        }

        /// <summary>Different filter options must all match.</summary>
        [Fact]
        public void Filters_Combine_As_Conjunction()
        {
            var args = new[] { "--filter-class", "Acme.OrderTests", "--filter-method", "Totals" };
            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Equal(2, result.Count);
        }

        /// <summary>Repeating one option matches any of its patterns.</summary>
        [Fact]
        public void Repeating_An_Option_Combines_As_Disjunction()
        {
            var args = new[] { "--filter-method", "Discounts", "--filter-method", "Totals" };
            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Equal(4, result.Count);
        }

        /// <summary>Patterns match without regard to case.</summary>
        [Fact]
        public void Matching_Is_Case_Insensitive()
        {
            var args = new[] { "--filter-method", "discounts" };
            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Single(result);
        }

        /// <summary>A consumed filter value is not handed back to the caller.</summary>
        [Fact]
        public void A_Filter_Value_Is_Not_Also_Left_In_The_Remaining_Arguments()
        {
            // The standalone runner treats any bare argument as a keyword filter. If the value of
            // --filter-name were left behind it would narrow the run a second time, by a different
            // rule, without ever saying so.
            var args = new[] { "--filter-name", "Discounts", "--other", "keep" };
            var expectedRemaining = new[] { "--other", "keep" };

            _ = TestFilter.Apply(Sample(), args, out var remaining);

            Assert.Equal(expectedRemaining, remaining);
        }

        /// <summary>A pattern is not reinterpreted as a regular expression.</summary>
        [Fact]
        public void Regex_Metacharacters_In_A_Pattern_Are_Matched_Literally()
        {
            // Data-row names routinely contain parentheses, so a pattern must not be reinterpreted
            // as a regular expression.
            var args = new[] { "--filter-name", "Totals(1)" };
            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Single(result);
            Assert.Equal("Acme.OrderTests.Totals(1)", result[0].UniqueName);
        }

        /// <summary>A filter matching nothing selects nothing rather than everything.</summary>
        [Fact]
        public void A_Filter_That_Matches_Nothing_Yields_No_Tests()
        {
            var args = new[] { "--filter-name", "NoSuchTest" };
            var result = TestFilter.Apply(Sample(), args, out _);

            Assert.Empty(result);
        }

        /// <summary>A registration without a structural name still filters.</summary>
        [Fact]
        public void Filtering_Falls_Back_To_FullName_When_UniqueName_Is_Absent()
        {
            // Registrations emitted by an older generator carry no UniqueName.
            var tests = new[]
            {
                new ProvaTest
                {
                    DisplayName = "Legacy",
                    FullName = "Acme.LegacyTests.Run",
                    ExecuteDelegate = () => Task.FromResult<string?>(null),
                },
            };

            var args = new[] { "--filter-name", "LegacyTests" };
            var result = TestFilter.Apply(tests, args, out _);

            Assert.Single(result);
        }
    }
}
