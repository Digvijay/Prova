using System.Collections.Generic;
using System.Globalization;
using FsCheck;

namespace Prova.FsCheck
{
    public static class FsCheckRunner
    {
        public static void Run(Dictionary<string, string>? configValues, Property property)
        {
            var config = Config.QuickThrowOnFailure;

            if (configValues != null)
            {
                if (TryGetBool(configValues, nameof(PropertyAttribute.Verbose), out var verbose) && verbose)
                    config = Config.VerboseThrowOnFailure;

                if (TryGetInt(configValues, nameof(PropertyAttribute.MaxTest), out var maxTest))
                    config = config.WithMaxTest(maxTest);

                if (TryGetInt(configValues, nameof(PropertyAttribute.MaxFail), out var maxFail))
                    config = config.WithMaxRejected(maxFail);

                if (TryGetInt(configValues, nameof(PropertyAttribute.StartSize), out var startSize))
                    config = config.WithStartSize(startSize);

                if (TryGetInt(configValues, nameof(PropertyAttribute.EndSize), out var endSize))
                    config = config.WithEndSize(endSize);

                if (TryGetBool(configValues, nameof(PropertyAttribute.QuietOnSuccess), out var quiet))
                    config = config.WithQuietOnSuccess(quiet);
            }

            Check.One(config, property);
        }

        private static bool TryGetInt(Dictionary<string, string> values, string key, out int result)
        {
            result = 0;
            return values.TryGetValue(key, out var raw)
                && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        private static bool TryGetBool(Dictionary<string, string> values, string key, out bool result)
        {
            result = false;
            return values.TryGetValue(key, out var raw) && bool.TryParse(raw, out result);
        }
    }
}
