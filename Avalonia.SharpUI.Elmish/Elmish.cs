using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace Avalonia.SharpUI.Elmish;

public static class Elmish
{
    public record ElmishItem(Control Control, object Updater);

    public static readonly List<ElmishItem> items = new();

    public static Program<TControl, TMsg> Init<TControl, TMsg, TState>(
        TControl control,
        IView<TMsg, TState> view,
        Func<TMsg, TState, TState> modelUpdater,
        TState initState
    )
        where TControl : ContentControl, new()
        where TState : IEquatable<TState>
    {
        var vu = new ViewUpdater<TMsg, TState>(control, view, modelUpdater, initState);
        var item = new ElmishItem(control, vu);
        items.Add(item);
        EventHandler<VisualTreeAttachmentEventArgs>? handler = null;
        control.DetachedFromVisualTree += handler = (s, e) =>
        {
            control.DetachedFromVisualTree -= handler;
            items.Remove(item);
        };

        vu.SetState(initState);
        return new(control, vu);
    }

    public static Control Create<TView, TState, TMsg>(
        this TView view,
        Func<TMsg, TState, TState> modelUpdater,
        TState initState)
        where TView : IView<TMsg, TState>
        where TState : IEquatable<TState>
    {
        var cnt = new ContentControl();
        var vu = new ViewUpdater<TMsg, TState>(cnt, view, modelUpdater, initState);
        vu.SetState(initState);
        return cnt;
    }
}
public record Program<TControl, TMsg>(TControl Control, IViewUpdater<TMsg> ViewUpdater)
    where TControl : ContentControl, new()
{
    public Program<TControl, TMsg> Inspect(Action<TControl, IViewUpdater<TMsg>> inspectAction)
    {
        inspectAction(Control, ViewUpdater);
        return this;
    }
}

public interface IViewUpdater<TMsg>
{
    void Invoke(TMsg msg);
};

public class ViewUpdater<TMsg, TState> : IViewUpdater<TMsg>
    where TState : IEquatable<TState>
{
    private readonly ContentControl parentControl;
    private readonly IView<TMsg, TState> view;
    private TState state;
    private readonly Func<TMsg, TState, TState> modelUpdater;

    public ViewUpdater(ContentControl control, IView<TMsg, TState> view, Func<TMsg, TState, TState> modelUpdater, TState initState)
    {
        this.parentControl = control;
        this.view = view;
        this.state = initState;
        this.modelUpdater = modelUpdater;
    }

    record ControlPath(int ChildIndex, Type ControlType);
    record FocusableInputKey(List<ControlPath> ControlPathes);
    internal void SetState(TState state)
    {
        this.state = state;
        var v = view.View(state, this);
        parentControl.Content = v;
    }

    public void Invoke(TMsg msg)
    {
        TState s = modelUpdater.Invoke(msg, this.state);
        if (!this.state.Equals(s))
        {
            SetState(s);
        }
    }
}

public interface IView<TMsg, TState>
{
    /// <summary>
    /// create new view instance;
    /// </summary>
    /// <param name="state">current view state.</param>
    /// <param name="viewUpdater">message dispatcher.</param>
    /// <returns></returns>
    Control View(TState state, IViewUpdater<TMsg> viewUpdater);
}



