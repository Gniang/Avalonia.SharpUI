using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Live.Avalonia;
using NumSharp;

namespace avalonia_play
{

    public class App : Application
    // ,ILiveView
    {
        public static readonly bool IsDebug = true;

        /// <summary>
        /// ウインドウインスタンスの作成
        /// </summary>
        public static Window CreateWindow<T>(string windowName = "") where T : IPageView, new()
        {
            return new Window
            {
                Name = windowName,
                Content = new T()
            };
        }

        public override void OnFrameworkInitializationCompleted()
        {
            this.Styles.Add(new FluentTheme());
            this.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
            var gridUri = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml");
            this.Styles.Add(new StyleInclude(gridUri) { Source = gridUri });
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window window = CreateWindow<MainPage>();
                desktop.MainWindow = window;
                MultiWindow.Init(window);
            }
            else
            {
                throw new Exception("Not supported platform.");
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

}