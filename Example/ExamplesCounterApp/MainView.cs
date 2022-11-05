using Avalonia.Controls;
using Avalonia.Media;
using static System.Net.Mime.MediaTypeNames;
using Avalonia.SharpUI;
using Avalonia.Layout;
using System.ComponentModel;

namespace ExamplesCounterApp
{
    internal class MainView : UserControl
    {
        private State state;

        /// <summary>
        /// TODO: ??? i need react like useState context？
        /// </summary>
        public class State : INotifyPropertyChanged
        {
            private int v;

            public int Value
            {
                get => v;
                set
                {
                    v = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
        public MainView()
        {

            this.state = new State();
            this.Content = new DockPanel()
                .Children(new Control[]
                {

                        new Button()
                        {
                            Content = "reset",
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                        }
                        .DockBottom()
                        .OnClick((s, e) => state.Value = 0)
                        ,
                        new Button()
                        {
                            Content = "-",
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                        .DockBottom()
                        .OnClick((s, e) => state.Value -= 1)
                        ,
                        new Button()
                        {
                            Content = "+",
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                        .DockBottom()
                        .OnClick((s, e) => state.Value += 1)
                        ,
                        new Button()
                        {
                            Content = "x2",
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                        .DockBottom()
                        .OnClick((s, e) => state.Value *= 2)
                        ,
                        new TextBox()
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                        .DockBottom()
                        .On(nameof(TextBox.TextChanged), (object? s, TextChangedEventArgs e) =>
                        {
                            if (int.TryParse(((TextBox)s).Text, out int i))
                            {
                                state.Value = i;
                            }
                        })
                        .SetBind(TextBox.TextProperty, state, nameof(State.Value))
                        ,
                        new TextBlock()
                        {
                            FontSize = 48.0,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                        .DockTop()
                        .SetBind(TextBlock.TextProperty, state, nameof(State.Value))
                        ,
                }
            );
        }
    }
}