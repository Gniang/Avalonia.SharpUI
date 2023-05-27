using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Live.Avalonia;

public interface IPageView
{
    object? Content { get; }

    string? Name { get; set; }
}
public class MultiWindow
{
    private static readonly List<Window> windows = new();
    private static Window? mainWindow = null;

    public static Window MainWindow() => mainWindow
        ?? throw new NullReferenceException($"Not initialized. Need [{nameof(MultiWindow)}.{nameof(Init)}(mainWindow)]");

    public static IReadOnlyList<Window> Windows => windows.AsReadOnly();

    public static T? Find<T>(string name) where T : IPageView
        => windows
            .Select(x => x.Content)
            .OfType<T>()
            .FirstOrDefault(view => view.Name == name);

    public static T? Find<T>(Func<T, bool> predicate) where T : IPageView
        => windows
            .Select(x => x.Content)
            .OfType<T>()
            .FirstOrDefault(predicate);

    public static T? Find<T>() where T : IPageView
        => windows
            .Select(x => x.Content)
            .OfType<T>()
            .FirstOrDefault();

    public static T Navigate<T>() where T : Window, new()
    {
        var window = new T();
        Navigate<T>(window);
        return window;
    }
    public static T Navigate<T>(T window) where T : Window
    {
        if (windows.Contains(window))
        {
            window.Activate();
            return window;
        }

        EventHandler? closedHandler = null;
        window.Closed += closedHandler = (s, e) =>
        {
            window.Closed -= closedHandler;
            windows.Remove((Window)s!);
        };

        windows.Add(window);
        window.Show();

        return window;
    }

    public static void Init(Window mainWindow)
    {
        MultiWindow.mainWindow = mainWindow;
        MultiWindow.windows.Add(mainWindow);
    }
}

public static class MultiWindowExtensions
{
    public static T Navigate<T>(this Control parent, T child) where T : Window
     => MultiWindow.Navigate(child);

    public static T? FindWindow<T>(this Control control) where T : IPageView
     => MultiWindow.Find<T>();

    public static T? FindWindow<T>(this Control control, Func<T, bool> predicate) where T : IPageView
     => MultiWindow.Find(predicate);

    public static T? FindWindow<T>(this Control control, string windowName) where T : IPageView
     => MultiWindow.Find<T>(windowName);
}