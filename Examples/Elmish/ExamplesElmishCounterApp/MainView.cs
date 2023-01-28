using Avalonia.Controls;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System;
using static ExamplesCounterApp.MainView;
using Avalonia.SharpUI.Elmish;

namespace ExamplesCounterApp;

internal class MainView : IView<Msg, State>
{
    public record State(int Count);

    public record Msg { };
    public record Increment() : Msg;
    public record Decrement() : Msg;
    public record SetCount(int Count) : Msg;
    public record Reset() : Msg;

    public static State Update(Msg msg, State s)
        => msg switch
        {
            Increment _ => s with { Count = s.Count + 1 },
            Decrement _ => s with { Count = s.Count - 1 },
            SetCount x => s with { Count = x.Count },
            Reset _ => s with { Count = 0 },
            _ => throw new Exception($"unexperimental msg.{msg}"),
        };



    public Control View(State state, IViewUpdater<Msg> v)
    {
        return new DockPanel()
            .Children(new Control[]
            {
                new Button()
                {
                    Content = "reset",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                }
                .DockBottom()
                .OnClick((s, e) => v.Invoke(new Reset()))
                ,
                new Button()
                {
                    Content = "-",
                    HorizontalAlignment = HorizontalAlignment.Stretch
                }
                .DockBottom()
                .OnClick((s, e) => v.Invoke(new Decrement()))
                ,
                new Button()
                {
                    Content = "+",
                    HorizontalAlignment = HorizontalAlignment.Stretch
                }
                .DockBottom()
                .OnClick((s, e) => v.Invoke(new Increment()))
                ,
                new Button()
                {
                    Content = "x2",
                    HorizontalAlignment = HorizontalAlignment.Stretch
                }
                .DockBottom()
                .OnClick((s, e) => v.Invoke(new SetCount(state.Count * 2)))
                ,
                new NumericUpDown()
                {
                    Value =state.Count,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                }
                .DockBottom()
                .On(nameof(NumericUpDown.ValueChanged), (object? s, NumericUpDownValueChangedEventArgs e) =>
                {
                    if (s is NumericUpDown n && DecimalToInt(n.Value) is int d)
                    {
                        v.Invoke(new SetCount(d));
                    }
                })
                ,
                new TextBlock()
                {
                    Text = $"{state.Count}",
                    FontSize = 48.0,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                }
                .DockTop()
                ,
            })
            ;
    }
    public static int? DecimalToInt(decimal? val)
    {
        if (val is not decimal d) return null;

        if (d > int.MaxValue || d < int.MinValue)
        {
            return null;
        }
        return decimal.ToInt32(d);
    }
}
