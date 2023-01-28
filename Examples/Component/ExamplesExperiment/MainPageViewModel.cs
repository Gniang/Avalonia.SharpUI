using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Dynamic;

namespace avalonia_play
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        private DataView? items = null;

        public MainPageViewModel()
        {
        }

        public DataView? Items
        {
            get => this.items;
            set
            {
                this.items = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        public string Text => "test color";

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}