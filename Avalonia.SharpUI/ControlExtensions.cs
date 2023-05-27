using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using System.Reactive;

namespace Avalonia.SharpUI;

public static class ControlExtensions
{
    /// <summary>
    /// bind data to avalonia property.
    /// <para>eg.</para>
    /// <para><code language="c#">
    /// new TextBox().SetBind(TextBox.TextProperty, viewModel, nameof(viewModel.Text))
    /// </code></para>
    /// <para>equals</para>
    /// <para><code> 
    /// &lt;Window.DataContext&gt;
    ///     &lt;local:ViewModel /&gt;
    /// &lt;/Window.DataContext&gt;
    /// &lt;TextBox Text="{Binding Text}" /&gt;
    /// </code></para>
    /// </summary>
    /// <param name="control">View control.</param>
    /// <param name="prorperty">binding property.</param>
    /// <param name="bindingItem">binding item.</param>
    /// <returns></returns>
    public static TControl SetBind<TControl>(this TControl control, AvaloniaProperty prorperty, IBinding bindingItem)
    where TControl : AvaloniaObject
    {
        control.Bind(prorperty, bindingItem);
        return control;
    }

    /// <inheritdoc cref="ControlExtensions.SetBind"/>
    /// <param name="bindingViewModel">Binding view model.</param>
    /// <param name="bindingPath">binding path in view model.</param>
    /// <param name="mode">binding mode.</param>
    public static TControl SetBind<TControl, TViewModel>(this TControl control,
                                                     AvaloniaProperty prorperty,
                                                     TViewModel bindingViewModel,
                                                     string bindingPath,
                                                     BindingMode mode = BindingMode.Default,
                                                     IValueConverter? converter = null)
    where TControl : AvaloniaObject
    {
        return SetBind(control, prorperty, new Binding(bindingPath, mode) { Source = bindingViewModel, Converter = converter });
    }

    /// <inheritdoc cref="ControlExtensions.SetBind"/>
    public static TControl SetBind<TControl, T>(this TControl control,
                                                AvaloniaProperty prorperty,
                                                IObservableState<T> bindingItem,
                                                BindingMode mode = BindingMode.Default,
                                                IValueConverter? converter = null)
    where TControl : AvaloniaObject
    {
        return SetBind(control, prorperty, new Binding(nameof(IObservableState<T>.Value), mode) { Source = bindingItem, Converter = converter });
    }

    public static TAvaloniaObject SetObservable<TAvaloniaObject, T>(this TAvaloniaObject avaloniaObject,
                                                     AvaloniaProperty<T> prorperty,
                                                     Action<IObservable<T>> action)
    where TAvaloniaObject : AvaloniaObject
    {
        // TODO: event leak;
        action.Invoke(avaloniaObject.GetObservable(prorperty));
        return avaloniaObject;
    }

    public static TAvaloniaObject SetSubscribe<TAvaloniaObject, T>(this TAvaloniaObject avaloniaObject,
                                                 AvaloniaProperty<T> prorperty,
                                                 Action<T> action)
    where TAvaloniaObject : AvaloniaObject
    {
        // TODO: event leak;
        avaloniaObject.GetObservable(prorperty).Subscribe(action);
        return avaloniaObject;
    }

    public static T OnClick<T>(this T button, Action<object?, RoutedEventArgs> action)
    where T : Button
    {
        button.Click += new EventHandler<RoutedEventArgs>(action);
        return button;
    }

    /// <summary>
    /// subscribe event.
    /// <para>
    /// <example>
    /// eg.
    /// <code language="C#">
    /// <para>
    ///   Button.Click += (s, e) => { Debug.WriteLine("Clicked"); };
    /// </para>
    ///      ↓
    /// <para>
    ///   Button.On(nameof(Button.Click), (object? s, RoutedEventArgs e) => { Debug.WriteLine("Clicked"); } );
    /// </para>
    /// </code>
    /// </example>
    /// </para>
    /// </summary>
    public static T On<T, TEvent>(this T control, string eventName, EventHandler<TEvent> eventHandler)
    where T : StyledElement
    where TEvent : EventArgs
    {
        var ev = typeof(T).GetEvent(eventName);
        if (ev is null)
        {
            throw new NullReferenceException($"[{typeof(T).FullName}] does not declared event [{eventName}].");
        }

        bool typeCompatible = ev.EventHandlerType?.IsAssignableFrom(typeof(EventHandler<TEvent>)) ?? false;
        if (!typeCompatible)
        {
            throw new MissingMemberException(
                $"[{eventName}] event hander type miss match. Actual type is [{ev.EventHandlerType?.FullName}], Argument type is [{typeof(EventHandler<TEvent>).FullName}]");
        }
        ev.AddEventHandler(control, eventHandler);
        return control;
    }


    public static Window? GetOwnerWindow(this StyledElement control)
    {
        var parent = control;
        while (parent != null)
        {
            if (parent is Window w)
            {
                return w;
            }
            parent = parent.Parent;
        }
        return null;
    }

    /// <summary>
    /// add child controls.
    /// </summary>
    /// <param name="panel">parent control</param>
    /// <param name="controls">child controls</param>
    /// <returns></returns>
    public static T Children<T>(this T panel, IEnumerable<Control> controls)
    where T : Panel
    {
        foreach (var c in controls)
        {
            panel.Children.Add(c);
        }
        return panel;
    }

    /// <inheritdoc cref="ControlExtensions.Children"/>
    public static T Children<T>(this T panel, params Control[] controls)
    where T : Panel
    {
        return Children(panel, controls.AsEnumerable());
    }

    /// <inheritdoc cref="ControlExtensions.Children"/>
    public static T Children<T>(this T panel, Func<T, IEnumerable<Control>> mapper)
    where T : Panel
    {
        var controls = mapper(panel);
        return Children(panel, controls);
    }

    public static T DockLeft<T>(this T control)
    where T : Control
    {
        DockPanel.SetDock(control, Dock.Left);
        return control;
    }

    public static T DockRight<T>(this T control)
        where T : Control
    {
        DockPanel.SetDock(control, Dock.Right);
        return control;
    }

    public static T DockBottom<T>(this T control)
    where T : Control
    {
        DockPanel.SetDock(control, Dock.Bottom);
        return control;
    }
    public static T DockTop<T>(this T control)
    where T : Control
    {
        DockPanel.SetDock(control, Dock.Top);
        return control;
    }

    // //
    // // Summary:
    // //     Gets or sets a parameter to be passed to the Avalonia.Controls.Button.Command.
    // public object? CommandParameter { get; set; }
    // //
    // // Summary:
    // //     Gets or sets an Avalonia.Input.KeyGesture associated with this control
    // public KeyGesture? HotKey { get; set; }
    // //
    // // Summary:
    // //     Gets or sets an System.Windows.Input.ICommand to be invoked when the button is
    // //     clicked.
    // public ICommand? Command { get; set; }
    // public static T Command<T>(this T control, IBinding binding)
    // where T : IControl
    // {
    //     control._set(() => control[!Button.CommandProperty] = binding);

    //     foreach (var c in controls)
    //     {
    //         panel.Children.Add(c);
    //     }
    //     return panel;
    // }


}
