using System;
using Serilog;

namespace PurpleFridayTweetListener.Logger
{
    public static class Logging
    {
        static Serilog.Core.Logger _logger;
        
        public static void SetupLogging(string filePath)
        {
            if (_logger == null)
            {
                if  (filePath == null)
                {
                    _logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();
                    _logger.Information("Creating Console Logger");                        
                }
                else
                {
                    _logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .WriteTo.File(filePath)
                        .CreateLogger();   

                    _logger.Information("Creating File Logger");
                }
            }
            else
            {
                _logger.Warning("SetupLogging called twice - Not recreating Logger!");
            }
        }
        public static void Debug(string message)
        {
            _logger.Debug(message);
        }

        public static void Information(string message)
        {
            _logger.Information(message);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public static void Warning(string message)
        {
            _logger.Warning(message);
        }
    }

}