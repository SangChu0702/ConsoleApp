using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace Core.Helpers
{
    public class LogHelper
    {
        private static bool _isInitialized = false;

        public static void Init(string logFilePath)
        {
            if (_isInitialized) return;

            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Async(a => a.File(formatter: new TextFormatter() , logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, shared: true))
                        .CreateLogger();

            _isInitialized = true;

            Log.Information("================================        START        ================================");
        }


        public static void Info(string message, [CallerMemberName] string method = "", [CallerFilePath] string file = "")
        {
            string className = Path.GetFileNameWithoutExtension(file);
            message = $"[{className}][{method}] - {message}";
            Log.Information(message);
        }


        public static void Warning(string message, [CallerMemberName] string method = "", [CallerFilePath] string file = "")
        {
            string className = Path.GetFileNameWithoutExtension(file);
            message = $"[{className}][{method}] - {message}";
            Log.Warning(message);
        }
        public static void Debug(string message, [CallerMemberName] string method = "", [CallerFilePath] string file = "")
        {
            string className = Path.GetFileNameWithoutExtension(file);
            message = $"[{className}][{method}] - {message}";
            Log.Debug(message);
        }

        public static void Error(string message, Exception? ex = null, [CallerMemberName] string method = "", [CallerFilePath] string file = "")
        {
            string className = Path.GetFileNameWithoutExtension(file);
            message = $"[{className}][{method}] - {message}";
            if (ex != null) message += " - ";
            Log.Error(ex, message);
        }

        public static void Exception(Exception ex, [CallerMemberName] string method = "", [CallerFilePath] string file = "")
        {
            string className = Path.GetFileNameWithoutExtension(file);
            var message = $"[{className}][{method}] - ";
            Log.Fatal(ex, message);
        }

        public static void Dispose()
        {
            Log.Information("================================         END         ================================");

            Log.CloseAndFlush();
        }

    }
    public class TextFormatter : Serilog.Formatting.ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            var level = logEvent.Level switch
            {
                LogEventLevel.Information => "INFO ",
                LogEventLevel.Fatal => "FATAL",
                LogEventLevel.Debug => "DEBUG",
                LogEventLevel.Warning => "WARN ",
                LogEventLevel.Error => "ERROR",
                _ => logEvent.Level.ToString().ToUpper()
            };

            output.Write($"[{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {logEvent.RenderMessage()}");

            if (logEvent.Exception != null)
            {
                output.Write(logEvent.Exception);
            }
            output.Write(Environment.NewLine);
        }
    }
}
