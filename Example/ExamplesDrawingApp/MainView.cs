using Avalonia.Controls;
using Avalonia.Media;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using Avalonia.Controls.Documents;
using Avalonia.Media.Immutable;
using System.Linq;

namespace ExamplesCounterApp;
internal class MainView : UserControl
{
    public MainView()
    {
        var brush = UseState(Brushes.Black as IBrush);
        var size = UseState(2);
        this.Content = new DockPanel
        {
            LastChildFill = true,
            Background = Brushes.White,
        }
            .Children(new Control[]
            {
                new SettingsView(brush, size)
                    {
                        Margin = new Avalonia.Thickness(5.0),
                        Padding = new Avalonia.Thickness(5.0),
                        CornerRadius = new Avalonia.CornerRadius(8.0),
                        Background = new ImmutableSolidColorBrush(Color.Parse("#bdc3c7")),
                    }
                    .DockBottom()
                ,
                new CanvasView(brush, size)
                    .DockTop()
                ,
            }
        );
    }
}

internal class CanvasView : UserControl
{
    public CanvasView(ObservableState<IBrush> brush, ObservableState<int> size)
    {
        var isPressed = UseState(false);
        var lastPoint = UseState(null as Avalonia.Point?);

        var canvas = new Canvas
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = Brushes.White,
        };
        this.Content = canvas;

        canvas
            .On(nameof(Canvas.PointerPressed),
                (object? s, Avalonia.Input.PointerPressedEventArgs e) => isPressed.Value = true
            )
            .On(nameof(Canvas.PointerReleased),
                (object? s, Avalonia.Input.PointerReleasedEventArgs e) =>
                {
                    isPressed.Value = false;
                    lastPoint.Value = null;
                }
            )
            .On(nameof(Canvas.PointerMoved),
                (object? s, Avalonia.Input.PointerEventArgs e) =>
                {
                    var pos = e.GetPosition(canvas);
                    if (isPressed.Value)
                    {
                        if (lastPoint.Value is Avalonia.Point lp)
                        {
                            var line = new Avalonia.Controls.Shapes.Line()
                            {
                                StartPoint = lp,
                                EndPoint = pos,
                                Stroke = brush.Value,
                                StrokeThickness = size.Value,
                                StrokeLineCap = PenLineCap.Round,
                            };
                            canvas.Children.Add(line);
                        }
                        lastPoint.Value = pos;
                    }
                }
            )
        ;
    }
}

internal class SettingsView : UserControl
{
    public SettingsView(ObservableState<IBrush> brush, ObservableState<int> size)
    {
        this.Content = new DockPanel
        {
            LastChildFill = false,
        }
        .Children(new Control[]{
            new ColorPickerView(brush)
                .DockLeft(),
            new SizePickerView(size)
                .DockRight(),
        })
        ;
    }
}

internal class ColorPickerView : UserControl
{
    public ColorPickerView(ObservableState<IBrush> brush)
    {
        var brushes = new[] {
            Brushes.Black,
            Brushes.Red,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Yellow,
        };

        this.Content = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5.0,
        }
        .Children(
            brushes
                .Select(item =>
                {
                    return new Border()
                    {
                        Width = 32.0,
                        Height = 32.0,
                        CornerRadius = new Avalonia.CornerRadius(16.0),
                        Background = item,
                        BorderThickness = new Avalonia.Thickness(4.0),
                    }
                    .SetBind(Border.BorderBrushProperty, brush, converter: ValueConverterSimple.OneWay(
                        (_) => brush.Value switch
                        {
                            IBrush b when b == item => item,
                            _ => Brushes.Transparent,
                        })
                    )
                    .On(nameof(Border.PointerPressed),
                        (object? s, Avalonia.Input.PointerPressedEventArgs e) => brush.Value = item
                    )
                    ;
                })
                .ToArray()
        )
        ;
    }
}

internal class SizePickerView : UserControl
{
    public SizePickerView(ObservableState<int> size)
    {
        var sizes = new[] {
            2,
            4,
            6,
            8,
            16,
            32,
        };

        this.Content = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5.0,
        }
        .Children(
            sizes
                .Select(item =>
                {
                    return new Border()
                    {
                        Width = item,
                        Height = item,
                        CornerRadius = new Avalonia.CornerRadius(item / 2.0f),
                    }
                    .SetBind(Border.BackgroundProperty, size, converter: ValueConverterSimple.OneWay(
                        (_) => size.Value switch
                        {
                            int s when s == item => Brushes.Black,
                            _ => Brushes.Gray,
                        })
                    )
                    .On(nameof(Border.PointerPressed),
                        (object? s, Avalonia.Input.PointerPressedEventArgs e) => size.Value = item
                    )
                    ;
                })
                .ToArray()
        )
        ;
    }
}