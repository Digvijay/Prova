using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.CommandLine;

namespace Prova
{
    /// <summary>
    /// Adds name-based test filtering to the Microsoft Testing Platform host.
    /// </summary>
    /// <remarks>
    /// The platform only offers <c>--filter-uid</c> out of the box, and a UID cannot be guessed:
    /// it has to be discovered by listing the tests first. Investigating a single failing test
    /// therefore meant running the whole project and reading the output. These options let a
    /// developer run one test, or one class, by name.
    /// </remarks>
    public sealed class ProvaFilterCommandLineProvider : ICommandLineOptionsProvider
    {
        /// <summary>The option that filters on the full test name.</summary>
        public const string FilterNameOption = "filter-name";

        /// <summary>The option that filters on the declaring class name.</summary>
        public const string FilterClassOption = "filter-class";

        /// <summary>The option that filters on the test method name.</summary>
        public const string FilterMethodOption = "filter-method";

        /// <inheritdoc />
        public string Uid => "Prova.Filter";

        /// <inheritdoc />
        public string Version => ProvaVersion.Value;

        /// <inheritdoc />
        public string DisplayName => "Prova test filtering";

        /// <inheritdoc />
        public string Description => "Filters tests by name, class or method, with '*' and '?' wildcards.";

        /// <inheritdoc />
        public Task<bool> IsEnabledAsync() => Task.FromResult(true);

        /// <inheritdoc />
        public IReadOnlyCollection<CommandLineOption> GetCommandLineOptions() => new[]
        {
            new CommandLineOption(
                FilterNameOption,
                "Run only tests whose full name matches the pattern. Supports '*' and '?'. May be repeated.",
                ArgumentArity.OneOrMore,
                false),
            new CommandLineOption(
                FilterClassOption,
                "Run only tests whose declaring class matches the pattern. Supports '*' and '?'. May be repeated.",
                ArgumentArity.OneOrMore,
                false),
            new CommandLineOption(
                FilterMethodOption,
                "Run only tests whose method name matches the pattern. Supports '*' and '?'. May be repeated.",
                ArgumentArity.OneOrMore,
                false),
        };

        /// <inheritdoc />
        public Task<ValidationResult> ValidateOptionArgumentsAsync(CommandLineOption commandOption, string[] arguments)
        {
            foreach (var argument in arguments)
            {
                if (string.IsNullOrWhiteSpace(argument))
                {
                    return ValidationResult.InvalidTask(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "'--{0}' requires a non-empty pattern.",
                            commandOption.Name));
                }
            }

            return ValidationResult.ValidTask;
        }

        /// <inheritdoc />
        public Task<ValidationResult> ValidateCommandLineOptionsAsync(ICommandLineOptions commandLineOptions)
            => ValidationResult.ValidTask;
    }

    /// <summary>
    /// Selects the tests that match the name filters supplied on the command line.
    /// </summary>
    /// <remarks>
    /// Both the platform-hosted runner and the standalone runner call into this type. Prova has
    /// previously shipped a feature on one of those paths and not the other; sharing the matching
    /// code is what stops the two from disagreeing.
    /// </remarks>
    public static class TestFilter
    {
        private static readonly string[] OptionNames =
        {
            ProvaFilterCommandLineProvider.FilterNameOption,
            ProvaFilterCommandLineProvider.FilterClassOption,
            ProvaFilterCommandLineProvider.FilterMethodOption,
        };

        /// <summary>
        /// Applies any name, class or method filter present in <paramref name="options"/>.
        /// </summary>
        /// <param name="tests">The discovered tests.</param>
        /// <param name="options">The parsed command line, or <see langword="null"/>.</param>
        /// <returns>
        /// The matching tests, or every test when no filter was supplied. A filter that matches
        /// nothing yields an empty sequence; the caller decides whether that is an error.
        /// </returns>
        public static IReadOnlyList<ProvaTest> Apply(IReadOnlyList<ProvaTest> tests, ICommandLineOptions? options)
        {
            if (options is null)
            {
                return tests;
            }

            return Apply(
                tests,
                GetPatterns(options, ProvaFilterCommandLineProvider.FilterNameOption),
                GetPatterns(options, ProvaFilterCommandLineProvider.FilterClassOption),
                GetPatterns(options, ProvaFilterCommandLineProvider.FilterMethodOption));
        }

        /// <summary>
        /// Applies any name, class or method filter present in a raw argument array, for hosts
        /// that never build a platform command line.
        /// </summary>
        /// <param name="tests">The discovered tests.</param>
        /// <param name="args">The raw process arguments.</param>
        /// <param name="remainingArguments">
        /// The arguments that were not consumed as filter options or their values. The caller must
        /// use these for any further argument handling, otherwise a filter's value is also read as
        /// a bare keyword and silently narrows the run a second time.
        /// </param>
        /// <returns>The matching tests, or every test when no filter was supplied.</returns>
        public static IReadOnlyList<ProvaTest> Apply(
            IReadOnlyList<ProvaTest> tests,
            string[] args,
            out string[] remainingArguments)
        {
            List<Regex>? namePatterns = null;
            List<Regex>? classPatterns = null;
            List<Regex>? methodPatterns = null;
            var remaining = new List<string>(args.Length);

            for (var i = 0; i < args.Length; i++)
            {
                var matched = false;

                foreach (var optionName in OptionNames)
                {
                    var prefix = "--" + optionName;

                    string? value = null;

                    if (args[i].StartsWith(prefix + "=", StringComparison.Ordinal))
                    {
                        value = args[i].Substring(prefix.Length + 1);
                    }
                    else if (string.Equals(args[i], prefix, StringComparison.Ordinal) && i + 1 < args.Length)
                    {
                        value = args[++i];
                    }

                    if (value is null)
                    {
                        continue;
                    }

                    matched = true;

                    var target = optionName == ProvaFilterCommandLineProvider.FilterNameOption
                        ? namePatterns ??= new List<Regex>()
                        : optionName == ProvaFilterCommandLineProvider.FilterClassOption
                            ? classPatterns ??= new List<Regex>()
                            : methodPatterns ??= new List<Regex>();

                    target.Add(ToRegex(value));
                    break;
                }

                if (!matched)
                {
                    remaining.Add(args[i]);
                }
            }

            remainingArguments = remaining.ToArray();

            return Apply(tests, namePatterns, classPatterns, methodPatterns);
        }

        private static IReadOnlyList<ProvaTest> Apply(
            IReadOnlyList<ProvaTest> tests,
            List<Regex>? namePatterns,
            List<Regex>? classPatterns,
            List<Regex>? methodPatterns)
        {
            if (namePatterns is null && classPatterns is null && methodPatterns is null)
            {
                return tests;
            }

            var result = new List<ProvaTest>();

            foreach (var test in tests)
            {
                if (namePatterns is not null && !MatchesAny(namePatterns, FullNameOf(test)))
                {
                    continue;
                }

                if (classPatterns is not null && !MatchesAny(classPatterns, ClassNameOf(test)))
                {
                    continue;
                }

                if (methodPatterns is not null && !MatchesAny(methodPatterns, MethodNameOf(test)))
                {
                    continue;
                }

                result.Add(test);
            }

            return result;
        }

        private static List<Regex>? GetPatterns(ICommandLineOptions options, string optionName)
        {
            if (!options.TryGetOptionArgumentList(optionName, out var arguments) || arguments is null || arguments.Length == 0)
            {
                return null;
            }

            return arguments.Select(ToRegex).ToList();
        }

        /// <summary>
        /// Translates a wildcard pattern into a regular expression.
        /// </summary>
        /// <remarks>
        /// Everything except '*' and '?' is escaped, so a pattern containing regex metacharacters
        /// — which test names routinely do, because generic arguments and data rows contain
        /// brackets and parentheses — matches literally rather than being reinterpreted.
        /// A pattern with no wildcard is treated as 'contains', which is what a developer typing
        /// a bare method name expects.
        /// </remarks>
        private static Regex ToRegex(string pattern)
        {
            var hasWildcard = pattern.Contains('*') || pattern.Contains('?');

            var builder = new StringBuilder();
            builder.Append(hasWildcard ? "^" : string.Empty);

            foreach (var c in pattern)
            {
                switch (c)
                {
                    case '*':
                        builder.Append(".*");
                        break;
                    case '?':
                        builder.Append('.');
                        break;
                    default:
                        builder.Append(Regex.Escape(c.ToString()));
                        break;
                }
            }

            builder.Append(hasWildcard ? "$" : string.Empty);

            return new Regex(builder.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        private static bool MatchesAny(List<Regex> patterns, string candidate)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.IsMatch(candidate))
                {
                    return true;
                }
            }

            return false;
        }

        private static string FullNameOf(ProvaTest test)
            => test.UniqueName ?? test.FullName ?? test.DisplayName;

        private static string ClassNameOf(ProvaTest test)
        {
            if (!string.IsNullOrEmpty(test.ClassName))
            {
                return test.ClassName!;
            }

            // Fall back to everything before the final '.' of the full name.
            var fullName = test.FullName ?? test.DisplayName;
            var lastDot = fullName.LastIndexOf('.');
            return lastDot > 0 ? fullName.Substring(0, lastDot) : fullName;
        }

        private static string MethodNameOf(ProvaTest test)
        {
            var fullName = test.FullName ?? test.DisplayName;

            // Trim any data-row or generic suffix so '--filter-method Foo' matches every row.
            var openParen = fullName.IndexOf('(');
            if (openParen > 0)
            {
                fullName = fullName.Substring(0, openParen);
            }

            var lastDot = fullName.LastIndexOf('.');
            return lastDot >= 0 && lastDot < fullName.Length - 1
                ? fullName.Substring(lastDot + 1)
                : fullName;
        }
    }
}
