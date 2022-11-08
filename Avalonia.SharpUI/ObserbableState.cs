using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;

namespace Avalonia.SharpUI;

public interface IObservableState<T>
{
    T Value { get; set; }
}

public interface IObservableState
{
    event PropertyChangedEventHandler? PropertyChanged;
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
