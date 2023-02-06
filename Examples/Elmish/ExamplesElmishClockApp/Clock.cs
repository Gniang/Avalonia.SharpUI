using Avalonia.Controls;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System;
using static ExamplesCounterApp.Clock;
using Avalonia.SharpUI.Elmish;
using Avalonia.Media;
using Avalonia.Controls.Shapes;

namespace ExamplesCounterApp;

internal class Clock : IView<Msg, State>
{
    public record State(DateTime Time);

    public static State Init() => new(DateTime.Now);

    public record Msg
    {
        private Msg() { }
        public record Tick(DateTime Time) : Msg;
    }

    public static State Update(Msg msg, State s)
        => msg switch
        {
            Msg.Tick t => s with { Time = t.Time },
            _ => throw new Exception($"unexperimental msg.{msg}"),
        };

    enum PointerType { Hour, Minute, Second }

    private Avalonia.Point CalcPointerPos(PointerType pointer, DateTime time)
    {
        double percent = pointer switch
        {
            PointerType.Hour => time.Hour / 12.0,
            PointerType.Minute => time.Minute / 60.0,
            PointerType.Second => time.Second / 60.0,
            _ => throw new Exception($"unexperimental pointer.{pointer}"),
        };

        double length = pointer switch
        {
            PointerType.Hour => 50.0,
            PointerType.Minute => 60.0,
            PointerType.Second => 70.0,
            _ => throw new Exception($"unexperimental pointer.{pointer}"),
        };

        double angle = 2.0 * Math.PI * percent;
        double handX = (100.0 + (length * Math.Cos(angle - (Math.PI / 2.0))));
        double handY = (100.0 + (length * Math.Sin(angle - (Math.PI / 2.0))));
        return new(handX, handY);
    }

    public Control View(State state, IViewUpdater<Msg> v)
    {
        return new Canvas()
        {
            Background = Brush.Parse("#2c3e50"),
        }
            .Children(new Control[]
            {
                new Ellipse()
                {
                    Width = 180,
                    Height = 180,
                    Fill = Brush.Parse("#ecf0f1")
                }
                .CanvasTop(10)
                .CanvasLeft(10)
                ,

                new Line(){
                    StartPoint = new(100,100),
                    EndPoint = CalcPointerPos(PointerType.Second, state.Time),
                    StrokeThickness = 2.0,
                    Stroke = Brush.Parse("#e74c3c"),
                }
                ,
                new Line(){
                    StartPoint = new(100,100),
                    EndPoint = CalcPointerPos(PointerType.Minute, state.Time),
                    StrokeThickness = 4.0,
                    Stroke = Brush.Parse("#7f8c8d"),
                }
                ,
                new Line(){
                    StartPoint = new(100,100),
                    EndPoint = CalcPointerPos(PointerType.Hour, state.Time),
                    StrokeThickness = 6.0,
                    Stroke = Brush.Parse("black"),
                }
                ,

                new Ellipse()
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brush.Parse("#95a5a6")
                }
                .CanvasTop(95)
                .CanvasLeft(95)
                ,
            })
            ;
    }
}
