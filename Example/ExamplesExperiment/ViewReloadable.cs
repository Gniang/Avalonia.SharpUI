
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Avalonia.Controls;
// using Live.Avalonia;

// public class ViewReloader : ILiveView
// {
//     private static readonly List<ReloadResrouce> resources = new List<ReloadResrouce>();

//     public static void Add<T>(Window window, object? dataContext = null) where T : IPageView
//     {
//         EventHandler? handler = null;
//         window.Closed += handler = (s, e) =>
//         {
//             window.Closed -= handler;
//             resources.RemoveAll(x => x.window == (Window)s!);
//         };

//         resources.Add(new ReloadResrouce(window, typeof(T), dataContext));
//     }

//     public object CreateView(Window window)
//     {
//         var r = resources.FirstOrDefault(x => x.window == window);
//         if (r == null)
//         {
//             throw new Exception("window missing");
//         }
//         if (r.window.DataContext == null)
//         {
//             r.window.DataContext = r.dataContext;
//         }

//         var content = (IPageView)Activator.CreateInstance(r.pageType)!;
//         content.Name = window.Name;
//         return content;
//     }
// }