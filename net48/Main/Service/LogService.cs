using Serilog;

using System;

namespace A.UI.Service
{
    internal static class LogService
    {
        public static void LoadConfiguration()
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            Log.Logger = new LoggerConfiguration()
                         .ReadFrom.AppSettings()
                         .CreateLogger();
        }

        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}
