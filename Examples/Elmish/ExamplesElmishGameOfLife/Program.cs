using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.SharpUI.Elmish;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using System;
using System.Diagnostics;

namespace ExamplesElmishGameOfLife;

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
                Title = "GameOfLife Example",
                Height = 500.0,
                Width = 500.0,
            };

            void updateBoard(IViewUpdater<MainView.Msg> vu)
            {
                DispatcherTimer.Run(() =>
                {
                    vu.Invoke(new MainView.Msg.BoardMsg(new Board.Msg.Evolve()));
                    return true;
                },
                TimeSpan.FromMilliseconds(100.0));
            }

            Elmish.Init(window, new MainView(), MainView.Update, MainView.Init())
                .Inspect((w, vu) =>
                {
                    w.Closed += (object? s, EventArgs e) => { Debug.WriteLine("The window has been closed."); };
                    updateBoard(vu);
                });

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = window;
            }
            base.OnFrameworkInitializationCompleted();
        }


    }
}
