using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FsCheck;

namespace Prova.FsCheck
{
    public static class FsCheckRunner
    {
        public static void Run(Dictionary<string, string>? configValues, Property property)
        {
            var config = Config.Default;
            if (configValues != null)
            {
                if (configValues.TryGetValue("MaxTest", out var mt) && int.TryParse(mt, out int maxTest)) config.MaxTest = maxTest;
                if (configValues.TryGetValue("MaxFail", out var mf) && int.TryParse(mf, out int maxFail)) config.MaxFail = maxFail;
                if (configValues.TryGetValue("StartSize", out var ss) && int.TryParse(ss, out int startSize)) config.StartSize = startSize;
                if (configValues.TryGetValue("EndSize", out var es) && int.TryParse(es, out int endSize)) config.EndSize = endSize;
                if (configValues.TryGetValue("Verbose", out var v) && bool.TryParse(v, out bool verbose) && verbose) config.Runner = ConsoleRunner.Verbose;
                if (configValues.TryGetValue("QuietOnSuccess", out var q) && bool.TryParse(q, out bool quiet) && quiet) config.QuietOnSuccess = true;
            }

            // Custom runner to capture failure and throw exception for Prova to catch
            config.Runner = new ProvaRunner(config.Runner);
            
            Check.One(config, property);
        }

        private class ProvaRunner : IRunner
        {
            private readonly IRunner _inner;

            public ProvaRunner(IRunner inner)
            {
                _inner = inner;
            }

            public void OnStartFixture(Type t) => _inner.OnStartFixture(t);

            public void OnArguments(int ntest, object[] args, Func<int, object[]> every)
            {
                // _inner.OnArguments(ntest, args, every); // concise output?
            }

            public void OnShrink(int ntest, object[] args) => _inner.OnShrink(ntest, args);

            public void OnFinished(string name, TestResult result)
            {
                _inner.OnFinished(name, result);
                if (result is TestResult.False f)
                {
                    // Throw exception to fail the test in Prova
                    throw new Exception($"Falsifiable, after {f.Item2.NbTests} tests ({f.Item2.NbShrinks} shrinks) : \n{Pretty.PrettyArgs(f.Item3)}");
                }
            }
        }
    }
}
