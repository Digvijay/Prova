using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Prova.Core.Tests
{
    /// <summary>
    /// Checks properties of the repository itself rather than of the code it contains.
    /// </summary>
    /// <remarks>
    /// Prova carries two solution files: <c>Prova.sln</c> for tooling that predates the XML format
    /// and <c>Prova.slnx</c> for tooling that prefers it. Nothing kept them in step, and they
    /// drifted — three projects were added to one and not the other. A project missing from a
    /// solution is not merely absent from the build; it is absent from the vulnerability audit,
    /// which is how a sample came to carry packages with published advisories that nobody saw.
    /// </remarks>
    public class SolutionParityTests
    {
        private static string RepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Prova.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new InvalidOperationException(
                $"Could not find the repository root above '{AppContext.BaseDirectory}'.");
        }

        private static string Normalise(string path)
            => path.Replace('\\', '/').Trim().TrimStart('.', '/');

        private static List<string> ProjectsInSln(string root)
        {
            var projects = new List<string>();

            foreach (var line in File.ReadLines(Path.Combine(root, "Prova.sln")))
            {
                if (!line.StartsWith("Project(", StringComparison.Ordinal))
                {
                    continue;
                }

                // Project("{guid}") = "Name", "relative\path.csproj", "{guid}"
                var fields = line.Split('=');
                if (fields.Length < 2)
                {
                    continue;
                }

                var path = fields[1]
                    .Split(',')
                    .Select(f => f.Trim().Trim('"'))
                    .FirstOrDefault(f => f.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase));

                if (path is not null)
                {
                    projects.Add(Normalise(path));
                }
            }

            return projects;
        }

        private static List<string> ProjectsInSlnx(string root)
        {
            var projects = new List<string>();

            foreach (var line in File.ReadLines(Path.Combine(root, "Prova.slnx")))
            {
                var marker = line.IndexOf("Path=\"", StringComparison.Ordinal);
                if (marker < 0)
                {
                    continue;
                }

                var start = marker + "Path=\"".Length;
                var end = line.IndexOf('"', start);
                if (end < 0)
                {
                    continue;
                }

                var path = line.Substring(start, end - start);
                if (path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    projects.Add(Normalise(path));
                }
            }

            return projects;
        }

        /// <summary>The two solution files list the same projects.</summary>
        [Fact]
        public void Both_Solution_Files_Contain_Every_Project()
        {
            var root = RepositoryRoot();

            var sln = ProjectsInSln(root);
            var slnx = ProjectsInSlnx(root);

            Assert.NotEmpty(sln);
            Assert.NotEmpty(slnx);

            var missingFromSlnx = sln.Except(slnx, StringComparer.OrdinalIgnoreCase).OrderBy(p => p, StringComparer.Ordinal).ToArray();
            var missingFromSln = slnx.Except(sln, StringComparer.OrdinalIgnoreCase).OrderBy(p => p, StringComparer.Ordinal).ToArray();

            Assert.True(
                missingFromSlnx.Length == 0,
                "Projects are in Prova.sln but not Prova.slnx: " + string.Join(", ", missingFromSlnx));

            Assert.True(
                missingFromSln.Length == 0,
                "Projects are in Prova.slnx but not Prova.sln: " + string.Join(", ", missingFromSln));
        }

        /// <summary>No project on disk is left out of the build and the audit.</summary>
        [Fact]
        public void Every_Project_In_The_Repository_Is_In_A_Solution()
        {
            var root = RepositoryRoot();

            var onDisk = Directory
                .EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(path => Normalise(Path.GetRelativePath(root, path)))
                .ToArray();

            var inSolution = ProjectsInSln(root);

            var orphans = onDisk.Except(inSolution, StringComparer.OrdinalIgnoreCase).OrderBy(p => p, StringComparer.Ordinal).ToArray();

            Assert.True(
                orphans.Length == 0,
                "Projects exist on disk but are in no solution, so they are neither built nor audited: "
                    + string.Join(", ", orphans));
        }
    }
}
