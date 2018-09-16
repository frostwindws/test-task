using System.IO;
using System.Web;
using Serilog;
using Serilog.Events;

namespace ArticlesService.Initialization
{
    /// <summary>
    /// Конфигурация библиотеки Serilog.
    /// </summary>
    internal static class SerilogConfiguration
    {
        /// <summary>
        /// Инициализация конфигурации.
        /// </summary>
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(Path.Combine(HttpRuntime.AppDomainAppPath, @"Logs\log-.txt"),
                    LogEventLevel.Verbose,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:HH:mm:ss:ffff} [{Level}]: {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}