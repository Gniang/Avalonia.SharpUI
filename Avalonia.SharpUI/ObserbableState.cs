using System;
using System.ComponentModel;
using System.Timers;

namespace Avalonia.SharpUI;

public interface IObservableState<T> : IObservableState
{
    T Value { get; set; }
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
}

public class ObservableState<T> : ObservableState, INotifyPropertyChanged, IObservableState<T>
{
    private T value;

    public ObservableState(T value)
    {
        this.value = value;
    }

    public T Value
    {
        get => value;
        set
        {
            this.value = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Value)));
        }
    }
}


public class DeferredState<T> : IObservableState<T>, IDisposable
{
    private readonly System.Timers.Timer timer = new();
    private readonly ElapsedEventHandler? handler;
    private T latestValue;

    public DeferredState(IObservableState<T> innerState, int deferredMillis)
    {
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
}
