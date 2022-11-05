// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using System.Linq;
// using System.Reactive.Concurrency;
// using System.Reactive.Linq;
// using System.Reactive.Subjects;
// using System.Reflection;
// using Avalonia.Controls;
// using Avalonia.Layout;
// using Avalonia.Threading;
// using Live.Avalonia;

// public sealed class LiveViewHost : Window, IDisposable
// {
//     private readonly LiveFileWatcher _assemblyWatcher;

//     private readonly LiveSourceWatcher _sourceWatcher;

//     private readonly IDisposable _subscription;

//     private readonly Action<string> _logger;

//     private readonly string _assemblyPath;

//     public LiveViewHost(ILiveView view, Action<string> logger)
//     {
//         _logger = logger;
//         _sourceWatcher = new LiveSourceWatcher(logger);
//         _assemblyWatcher = new LiveFileWatcher(logger);
//         _assemblyPath = view.GetType().Assembly.Location;
//         LiveControlLoader loader = new LiveControlLoader(logger);
//         _subscription = _assemblyWatcher.FileChanged.ObserveOn((IScheduler)AvaloniaScheduler.Instance).Subscribe(delegate (string path)
//         {
//             loader.LoadControl(path, this);
//         });
//         Console.CancelKeyPress += delegate
//         {
//             Clean("Console Ctrl+C key press.", exception: false);
//         };
//         AppDomain.CurrentDomain.ProcessExit += delegate
//         {
//             Clean("Process termination.", exception: false);
//         };
//         AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
//         {
//             Clean(args.ExceptionObject.ToString(), exception: true);
//         };
//     }

//     public void StartWatchingSourceFilesForHotReloading()
//     {
//         _logger("Starting source and assembly file watchers...");
//         var (dir, filePath) = _sourceWatcher.StartRebuildingAssemblySources(_assemblyPath);
//         _assemblyWatcher.StartWatchingFileCreation(dir, filePath);
//     }

//     public void Dispose()
//     {
//         _logger("Disposing LiveViewHost internals...");
//         _sourceWatcher.Dispose();
//         _assemblyWatcher.Dispose();
//         _subscription.Dispose();
//         _logger("Successfully disposed LiveViewHost internals.");
//     }

//     private void Clean(string reason, bool exception)
//     {
//         _logger("Closing live reloading window due to: " + reason);
//         if (exception)
//         {
//             _logger("\nNote: To prevent your app from crashing, properly handle all exceptions causing a crash.\nIf you are using ReactiveUI and ReactiveCommand<,>s, make sure you subscribe to RxApp.DefaultExceptionHandler: https://reactiveui.net/docs/handbook/default-exception-handler\nIf you are using another framework, refer to its docs considering global exception handling.\n");
//         }
//         Dispose();
//         Process.GetCurrentProcess().Kill();
//     }
// }

// internal sealed class LiveControlLoader
// {
//     private readonly Action<string> _logger;

//     public LiveControlLoader(Action<string> logger)
//     {
//         _logger = logger;
//     }

//     public void LoadControl(string assemblyPath, Window window)
//     {
//         try
//         {
//             _logger("Loading assembly from " + assemblyPath);
//             Assembly assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
//             _logger("Obtaining ILiveView interface implementation...");
//             Type interfaceType = typeof(ILiveView);
//             List<Type> list = (from type in assembly.GetTypes()
//                                where interfaceType.IsAssignableFrom(type)
//                                select type).ToList();
//             if (list.Count == 0)
//             {
//                 throw new TypeLoadException("No ILiveView interface implementations found in " + assemblyPath);
//             }
//             if (list.Count > 1)
//             {
//                 throw new TypeLoadException("Multiple ILiveView interface implementations found in " + assemblyPath);
//             }
//             _logger("Successfully managed to obtain ILiveView interface implementation, activating...");
//             Type type2 = list.First();
//             object obj = Activator.CreateInstance(type2);
//             string name = "CreateView";
//             MethodInfo methodInfo = type2.GetMethod(name) ?? interfaceType.GetMethod(name);
//             if (methodInfo == null)
//             {
//                 throw new TypeLoadException("Unable to obtain CreateView method!");
//             }
//             _logger("Successfully managed to obtain the method CreateView, creating control.");
//             window.Content = methodInfo.Invoke(obj, new object[1] { window });
//         }
//         catch (Exception ex)
//         {
//             window.Content = new TextBlock
//             {
//                 HorizontalAlignment = HorizontalAlignment.Center,
//                 VerticalAlignment = VerticalAlignment.Center,
//                 Text = ex.ToString()
//             };
//         }
//     }
// }

// internal sealed class LiveFileWatcher : IDisposable
// {
//     private readonly Subject<string> _fileChanged = new Subject<string>();

//     private readonly Action<string> _logger;

//     private readonly FileSystemWatcher watcher = new FileSystemWatcher
//     {
//         EnableRaisingEvents = false
//     };

//     public IObservable<string> FileChanged => _fileChanged.Throttle(TimeSpan.FromSeconds(0.5));

//     public LiveFileWatcher(Action<string> logger)
//     {
//         _logger = logger;
//     }

//     public void StartWatchingFileCreation(string dir, string filePath)
//     {
//         try
//         {
//             _logger("Registering observable file system watcher for file at: " + filePath);
//             watcher.Path = dir;
//             watcher.Filter = Path.GetFileName(filePath);
//             watcher.Changed += new FileSystemEventHandler(OnChanged);
//             watcher.IncludeSubdirectories = true;
//             watcher.EnableRaisingEvents = true;
//         }
//         catch (Exception value)
//         {
//             Console.WriteLine(value);
//         }
//     }

//     private void OnChanged(object sender, FileSystemEventArgs args)
//     {
//         _fileChanged.OnNext(args.FullPath);
//     }

//     public void Dispose()
//     {
//         _logger("Stopping the file creation watcher timer...");
//         _fileChanged.Dispose();
//         watcher.Changed -= new FileSystemEventHandler(OnChanged);
//         watcher.Dispose();
//     }
// }

// internal class LiveSourceWatcher : IDisposable
// {
//     private readonly Action<string> _logger;

//     private Process _dotnetWatchBuildProcess;

//     public LiveSourceWatcher(Action<string> logger)
//     {
//         _logger = logger;
//     }

//     public (string dir, string file) StartRebuildingAssemblySources(string assemblyPath)
//     {
//         _logger("Attempting to run 'dotnet watch' command for assembly sources...");
//         string text = FindAscendantDirectory(assemblyPath, "bin");
//         string directoryName = Path.GetDirectoryName(text);
//         if (directoryName == null)
//         {
//             throw new IOException("Unable to parent directory of " + text);
//         }
//         _logger("Preparing .live-bin directory...");
//         string text2 = Path.Combine(text, ".live-bin") + Path.DirectorySeparatorChar;
//         if (Directory.Exists(text2))
//         {
//             Directory.Delete(text2, recursive: true);
//             Directory.CreateDirectory(text2);
//         }
//         _logger("Executing 'dotnet watch' command from " + directoryName + ", building into " + text2);
//         _dotnetWatchBuildProcess = new Process
//         {
//             StartInfo = new ProcessStartInfo
//             {
//                 FileName = "dotnet",
//                 Arguments = "watch msbuild /p:BaseOutputPath=" + text2,
//                 UseShellExecute = true,
//                 CreateNoWindow = true,
//                 RedirectStandardOutput = false,
//                 RedirectStandardError = false,
//                 WorkingDirectory = directoryName
//             }
//         };
//         _dotnetWatchBuildProcess.Start();
//         _logger($"Successfully managed to start 'dotnet watch' process with id {_dotnetWatchBuildProcess.Id}");
//         char directorySeparatorChar = Path.DirectorySeparatorChar;
//         string item = assemblyPath.Replace($"{directorySeparatorChar}bin{directorySeparatorChar}", $"{directorySeparatorChar}bin{directorySeparatorChar}.live-bin{directorySeparatorChar}");
//         return (text2, item);
//     }

//     public void Dispose()
//     {
//         if (_dotnetWatchBuildProcess != null)
//         {
//             _logger($"Killing 'dotnet watch' process {_dotnetWatchBuildProcess.Id} and dependent processes...");
//             _dotnetWatchBuildProcess.Kill(entireProcessTree: true);
//         }
//     }

//     private static string FindAscendantDirectory(string filePath, string directoryName)
//     {
//         string text = filePath;
//         DirectoryInfo directoryInfo;
//         do
//         {
//             text = Path.GetDirectoryName(text);
//             if (text == null)
//             {
//                 throw new IOException("Unable to get parent directory of " + filePath + " named " + directoryName);
//             }
//             directoryInfo = new DirectoryInfo(text);
//         }
//         while (!(directoryName == directoryInfo.Name));
//         return text;
//     }
// }