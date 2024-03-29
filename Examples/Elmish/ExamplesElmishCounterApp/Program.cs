using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.SharpUI.Elmish;
using Avalonia.Themes.Fluent;
using System;

namespace ExamplesCounterApp;

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
                Title = "Counter Example",
                Height = 400.0,
                Width = 400.0,
            };

            Elmish.Init(window, new MainView(), MainView.Update, new MainView.State(0));

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = window;
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
