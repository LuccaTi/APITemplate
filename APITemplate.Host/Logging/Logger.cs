using Serilog;
using ILogger = Serilog.ILogger;

namespace APITemplate.Host.Logging
{
    internal static class Logger
    {
        #region Attributes
        private const string _className = "Logger";
        private static ILogger? _logger;
        #endregion

        #region Methods
        internal static void InitLogger(string logDirectory)
        {
            try
            {
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                _logger = new LoggerConfiguration()
                      .MinimumLevel.Debug()
                      .WriteTo.Console()
                      .WriteTo.File(Path.Combine(logDirectory, $"system_log_.txt"),
                      rollingInterval: RollingInterval.Day, // One log file per day
                      retainedFileCountLimit: null, // Null keeps files indefinitely
                      shared: true // Allows real-time log writing monitoring
                      )
                      .CreateLogger();

                Log.Logger = _logger;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static void Debug(string className, string methodName, string message)
        {
            try
            {
                _logger!.Debug($"{className} - {methodName} - {message}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static void Info(string message)
        {
            try
            {
                _logger!.Information(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static void Error(string className, string methodName, string message)
        {
            try
            {
                _logger!.Error($"{className} - {methodName} - {message}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
