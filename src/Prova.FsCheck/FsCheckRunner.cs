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
            // For now, use the simplest way to run FsCheck from C# that throws on failure.
            // FsCheck 2.x configuration from C# is complex due to F# record types.
            Check.QuickThrowOnFailure(property);
        }
    }
}
