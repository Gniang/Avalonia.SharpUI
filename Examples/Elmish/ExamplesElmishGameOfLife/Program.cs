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
            this.Styles.Add(new FluentTheme(new Uri("avares://ControlCatalog/Styles")) { Mode = FluentThemeMode.Light });
            Window window = new Window
            {
                Title = "GameOfLife Example",
                Height = 500.0,
                Width = 500.0,
            };

            void updateBoard(IViewUpdater<Board.Msg> vu)
            {
                DispatcherTimer.Run(() =>
                {
                    //vu.Invoke(new Board.Msg.BoardMsg(DateTime.Now));
                    return true;
                },
                TimeSpan.FromMilliseconds(1000.0));
            }

            Elmish.Init(window, new Board(), Board.Update, Board.Init())
                .Inspect((w,  vu) =>
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
