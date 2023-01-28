using Avalonia.Controls;
using Avalonia.Input;
using ExamplesCounterApp;
using System;
using System.Collections.Generic;
using static ExamplesCounterApp.MainView;

namespace Avalonia.SharpUI.Elmish;

public class Elmish
{
    public record ElmishItem(IControl Control, object Updater);

    public static readonly List<ElmishItem> items = new();

    public static TControl Init<TControl, TMsg, TState>(TControl control,
                                                    IView<TMsg, TState> view,
                                                    Func<TMsg, TState, TState> modelUpdater,
                                                    TState initState
                                                )
        where TControl : ContentControl
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
        return control;
    }
}

public interface IViewUpdater<TMsg>
{
    void Invoke(TMsg msg);
};

public class ViewUpdater<TMsg, TState> : IViewUpdater<TMsg>
    where TState : IEquatable<TState>
{
    private readonly ContentControl control;
    private readonly IView<TMsg, TState> view;
    private TState state;
    private readonly Func<TMsg, TState, TState> modelUpdater;

    public ViewUpdater(ContentControl control, IView<TMsg, TState> view, Func<TMsg, TState, TState> modelUpdater, TState initState)
    {
        this.control = control;
        this.view = view;
        this.state = initState;
        this.modelUpdater = modelUpdater;
    }

    internal void SetState(TState state)
    {
        this.state = state;
        var focused = FocusManager.Instance?.Current;
        control.Content = view.View(state, this);
        if (focused != null)
        {
            var aa = 1;
        }
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
    IControl View(TState state, IViewUpdater<TMsg> viewUpdater);
}



