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

public class Elmish
{
    public record ElmishItem(IControl Control, object Updater);

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

    record ControlPath(int ChildIndex, Type ControlType);
    record FocusableInputKey(List<ControlPath> ControlPathes);
    internal void SetState(TState state)
    {
        this.state = state;
        var focused = FocusManager.Instance?.Current;
        var key = GetVisualKey(focused);
        var v = view.View(state, this);
        control.Content = v;

        // visual tree is created after to load
        //var t = Dispatcher.UIThread.InvokeAsync(() =>
        //{
        //    if (focused != null && key != null)
        //    {
        //        var newFocusing = SearchVisual(this.control, key);
        //        newFocusing?.Focus();
        //        if (focused is TextBox prevT && newFocusing is TextBox newT)
        //        {
        //            newT.CaretIndex = prevT.CaretIndex;
        //        }
        //    }
        //}, DispatcherPriority.Loaded);
        EventHandler<RoutedEventArgs>? handler = null;
        v.Loaded += handler = (s, e) =>
        {
            v.Loaded -= handler;
            if (focused != null && key != null)
            {
                var newFocusing = SearchVisual(this.control, key);
                newFocusing?.Focus();
                if (focused is TextBox prevT && newFocusing is TextBox newT)
                {
                    newT.CaretIndex = prevT.CaretIndex;
                }
            }
        };
    }


    [return: NotNullIfNotNull(nameof(item))]
    private FocusableInputKey? GetVisualKey(IInputElement? item)
    {
        if (item is null)
        {
            return null;
        }
        VisualTree.IVisual? prev = item;
        VisualTree.IVisual? parent = item.VisualParent;
        var keys = new List<ControlPath>(100);
        while (parent != null)
        {
            var i = IndexOf(parent.VisualChildren, x => x == prev);
            keys.Add(new ControlPath(i, prev.GetType()));
            if (parent == this.control) break;
            prev = parent;
            parent = parent.VisualParent;
        }
        keys.Reverse();

        return new FocusableInputKey(keys);
    }

    private IInputElement? SearchVisual(VisualTree.IVisual visual, FocusableInputKey key)
    {
        return SearchVisualSub(visual, CollectionsMarshal.AsSpan(key.ControlPathes));
    }

    private IInputElement? SearchVisualSub(IVisual visual, Span<ControlPath> pathes)
    {
        if (pathes.Length == 0)
        {
            return null;
        }

        var vcs = visual.VisualChildren;
        if (!(pathes[0] is ControlPath p && p.ChildIndex < vcs.Count))
        {
            return null;
        }
        var child = vcs[p.ChildIndex];
        if (child.GetType() != p.ControlType)
        {
            return null;
        }

        if (pathes.Length == 1 && child is IInputElement iv)
        {
            return iv;
        }
        return SearchVisualSub(child, pathes[1..]);
    }


    private static int IndexOf<T>(IReadOnlyList<T> self, Func<T, bool> predicate)
    {
        var c = self.Count;
        for (int i = 0; i < c; ++i)
        {
            if (predicate(self[i]))
                return i;
        }

        return -1;
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



