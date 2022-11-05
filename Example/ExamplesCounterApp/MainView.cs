using Avalonia.Controls;
using Avalonia.Media;
using static System.Net.Mime.MediaTypeNames;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using System.ComponentModel;
using System.Reactive.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace ExamplesCounterApp
{
    internal class MainView : UserControl
    {
        public MainView()
        {
            ObservableState<int> state = UseState(0);
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
                        if (int.TryParse((s as TextBox)?.Text, out int i))
                        {
                            state.Value = i;
                        }
                    })
                    .SetBind(TextBox.TextProperty, state)
                    ,
                    new TextBlock()
                    {
                        FontSize = 48.0,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    }
                    .DockTop()
                    .SetBind(TextBlock.TextProperty, state)
                    ,
                }
            );
        }

    }
}