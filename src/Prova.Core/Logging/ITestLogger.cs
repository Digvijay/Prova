namespace Prova.Logging
{
    /// <summary>
    /// Defines a mechanism for logging messages during test execution.
    /// </summary>
    public interface ITestLogger
    {
        /// <summary>Logs a general message.</summary>
        void Log(string message);

        /// <summary>Logs a warning message.</summary>
        void LogWarning(string message);

        /// <summary>Logs an error message.</summary>
        void LogError(string message);
    }
}
