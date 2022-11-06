using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamplesCounterApp
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseSkia()
                .StartWithClassicDesktopLifetime(args);
        }

        public class App : Application
        {
            public override void OnFrameworkInitializationCompleted()
            {
                this.Styles.Add(new FluentTheme(baseUri: null!) { Mode = FluentThemeMode.Dark });
                Window window = new Window
                {
                    Title = "Drawing App",
                    Width = 500.0,
                    Height = 500.0,
                    Content = new MainView(),
                };
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = window;
                }
                base.OnFrameworkInitializationCompleted();
            }
        }
    }
}