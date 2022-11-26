using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace Avalonia.SharpUI;


public interface IReadOnlyState<T>
{
    T Value { get; }
}
public interface IWritableState<T>
{
    T Value { set; }
}
public interface IObservableState<T> : IObservableState, IReadOnlyState<T>, IWritableState<T>
{
    new T Value { get; set; }
    IBinding ToBinding(BindingMode mode = BindingMode.Default, IValueConverter? converter = null);
}

public interface IObservableState : INotifyPropertyChanged
{
}

public class ObservableState : IObservableState
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    public static ObservableState<T> UseState<T>(T v)
    {
        return new ObservableState<T>(v);
    }

    public static IObservableState<T> UseDeferred<T>(IObservableState<T> state, int deferredMilis)
    {
        return new DeferredState<T>(state, deferredMilis);
    }


    public static void UseEffect(Action effectAction, params IObservableState[] triggerStates)
    {
        foreach (var s in triggerStates)
        {
            s.PropertyChanged += (s, e) =>
            {
                effectAction();
            };
        }
    }


    public static IBinding ToMultiBinding(IEnumerable<object> states, BindingMode mode, IMultiValueConverter converter)
    {
        return new MultiBinding()
        {
            Bindings = states.Select(x =>
            {
                return x switch
                {
                    IBinding b => b,
                    IObservableState s when s.GetType().IsGenericType => new Binding(nameof(IObservableState<object>.Value)) { Source = s },
                    _ => new Binding() { Source = x },
                };
            })
            .ToArray(),
            Mode = mode,
            Converter = converter,
        };
    }
}


public class ObservableState<T> : ObservableState, INotifyPropertyChanged, IObservableState<T>
{
    private T value;

    public ObservableState(T value)
    {
        this.value = value;
    }

    public virtual T Value
    {
        get => value;
        set
        {
            this.value = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Value)));
        }
    }
    public virtual IBinding ToBinding(BindingMode mode = BindingMode.Default, IValueConverter? converter = null)
    {
        return new Binding(nameof(Value)) { Source = this, Converter = converter, Mode = mode };
    }
}



public class DeferredState<T> : IObservableState<T>, IDisposable
{
    private readonly System.Timers.Timer timer = new();
    private readonly ElapsedEventHandler? handler;
    private readonly IObservableState<T> innerState;
    private T latestValue;

    public DeferredState(IObservableState<T> innerState, int deferredMillis)
    {
        this.innerState = innerState;
        this.latestValue = innerState.Value;
        timer.Interval = deferredMillis;
        timer.Elapsed += handler = (s, e) =>
        {
            timer.Stop();
            innerState.Value = latestValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        };
    }

    public T Value
    {
        get => latestValue;
        set
        {
            latestValue = value;
            timer.Stop();
            timer.Start();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose()
    {
        timer.Elapsed -= handler;
        timer.Stop();
        timer.Dispose();
    }

    public IBinding ToBinding(BindingMode mode = BindingMode.Default, IValueConverter? converter = null)
    {
        return innerState.ToBinding(mode, converter);
    }
}
