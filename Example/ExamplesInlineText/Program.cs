using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.OpenGL;
using Avalonia.Themes.Fluent;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                .StartWithClassicDesktopLifetime(args);
        }

        public class App : Application
        {
            public override void OnFrameworkInitializationCompleted()
            {
                this.Styles.Add(new FluentTheme(new Uri("avares://ControlCatalog/Styles")) { Mode = FluentThemeMode.Light });
                Window window = new Window
                {
                    Title = "Examples InlineText",
                    Width = 1200.0,
                    Height = 400.0,
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