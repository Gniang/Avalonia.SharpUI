using Avalonia;
using System;
using Avalonia.ReactiveUI;
using NLog.Fluent;
using NLog;
using System.Diagnostics;

namespace avalonia_play
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            LogManager.Configuration = new NLog.Config.LoggingConfiguration();
            {
                LogManager.Configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, new NLog.Targets.ConsoleTarget("logConsole"));
                LogManager.Configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, new NLog.Targets.FileTarget("logFile") { FileName = "log.txt" });
            }
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                // here we can work with the exception, for example add it to our log file
                logger.Fatal(e, "Something very bad happened");
                Debug.WriteLine(e.ToString());
            }
            finally
            {
                // This block is optional. 
                // Use the finally-block if you need to clean things up or similar
                LogManager.Flush();
            }
        }
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                // .UseReactiveUI()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
