using Avalonia.Controls;
using Avalonia.Controls.Shapes;

namespace Avalonia.SharpUI;

public static class CanvasExtensions
{
    public static TCanvasObject CanvasLeft<TCanvasObject>(this TCanvasObject shape, double left)
    where TCanvasObject : AvaloniaObject
    {
        Canvas.SetLeft(shape, left);
        return shape;
    }

    public static TCanvasObject CanvasTop<TCanvasObject>(this TCanvasObject shape, double top)
    where TCanvasObject : AvaloniaObject
    {
        Canvas.SetTop(shape, top);
        return shape;
    }

    public static TCanvasObject CanvasBottom<TCanvasObject>(this TCanvasObject shape, double bottom)
    where TCanvasObject : AvaloniaObject
    {
        Canvas.SetBottom(shape, bottom);
        return shape;
    }

    public static TCanvasObject CanvasRight<TCanvasObject>(this TCanvasObject shape, double right)
    where TCanvasObject : AvaloniaObject
    {
        Canvas.SetRight(shape, right);
        return shape;
    }
}
