using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.SharpUI.Elmish;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using System;
using System.Diagnostics;

namespace ExamplesClockApp;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args);
    }

    public class App : Application
    {
        public override void OnFrameworkInitializationCompleted()
        {
            this.Styles.Add(new FluentTheme());
            this.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
            Window window = new Window
            {
                Title = "Clock Example",
                Height = 400.0,
                Width = 400.0,
            };

            void updateClock(IViewUpdater<Clock.Msg> vu)
            {
                DispatcherTimer.Run(() =>
                {
                    vu.Invoke(new Clock.Msg.Tick(DateTime.Now));
                    return true;
                },
                TimeSpan.FromMilliseconds(1000.0));
            }

            Elmish.Init(window, new Clock(), Clock.Update, Clock.Init())
                .Inspect((w, vu) =>
                {
                    w.Closed += (s, e) => { Debug.WriteLine("The window has been closed."); };
                    updateClock(vu);
                });

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = window;
            }
            base.OnFrameworkInitializationCompleted();
        }


    }
}
