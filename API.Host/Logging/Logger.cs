using Serilog;
using ILogger = Serilog.ILogger;

namespace Service.API.Logging
{
    internal static class Logger
    {
        #region Atributes
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
                                    rollingInterval: RollingInterval.Day, // Um arquivo de log por dia
                                    retainedFileCountLimit: null, // Null mantém os arquivos indefinidamente
                                    shared: true // Permite acompanhar em tempo real a escrita no log
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
        internal static void Info(string className, string methodName, string message)
        {
            try
            {
                _logger!.Information($"{className} - {methodName} - {message}");
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
