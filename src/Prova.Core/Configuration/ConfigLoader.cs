using System;
using System.IO;
using System.Text.Json;

namespace Prova.Configuration
{
    /// <summary>
    /// Utility for loading Prova configuration from testconfig.json.
    /// </summary>
    public static class ConfigLoader
    {
        private const string ConfigFileName = "testconfig.json";

        /// <summary>
        /// Loads the configuration from the current directory.
        /// </summary>
        /// <returns>The loaded <see cref="ProvaConfig"/> or a default instance if not found.</returns>
        public static ProvaConfig Load()
        {
            try
            {
                string configPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    return JsonSerializer.Deserialize(json, ProvaConfigJsonContext.Default.ProvaConfig) ?? new ProvaConfig();
                }
            }
            catch (Exception ex)
            {
                // Fallback to default if config loading fails. 
                // We don't want to crash the test run because of a malformed config file.
                Console.WriteLine($"[Prova] Warning: Failed to load {ConfigFileName}: {ex.Message}");
            }

            return new ProvaConfig();
        }
    }
}
