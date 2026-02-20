using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Prova.Configuration
{
    /// <summary>
    /// Represents the configuration for Prova test execution, typically loaded from testconfig.json.
    /// </summary>
    public class ProvaConfig
    {
        /// <summary>
        /// Gets or sets the default number of times a test should be retried upon failure.
        /// </summary>
        public int? DefaultRetryCount { get; set; }

        /// <summary>
        /// Gets or sets the default timeout for each test in milliseconds.
        /// </summary>
        public int? DefaultTimeoutMs { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of tests to run in parallel.
        /// </summary>
        public int? MaxParallel { get; set; }

        /// <summary>
        /// Gets or sets the default culture for tests.
        /// </summary>
        public string? DefaultCulture { get; set; }

        /// <summary>
        /// Gets or sets global properties that apply to all tests.
        /// </summary>
        public Dictionary<string, string> GlobalProperties { get; set; } = new();
    }

    [JsonSerializable(typeof(ProvaConfig))]
    internal sealed partial class ProvaConfigJsonContext : JsonSerializerContext
    {
    }
}
