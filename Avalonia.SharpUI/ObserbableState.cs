using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;

namespace Avalonia.SharpUI
{
    public interface IObservableState<T>
    {
        T Value { get; set; }
    }

    public static class ObservableState
    {
        public static ObservableState<T> UseState<T>(T v)
        {
            return new ObservableState<T>(v);
        }
    }

    public class ObservableState<T> : INotifyPropertyChanged, IObservableState<T>
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
