using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Avalonia.SharpUI;

public class ValueConverterSimple : IValueConverter
{
    private readonly Func<object?, object?> conv;
    private readonly Func<object?, object?>? convBack;

    public ValueConverterSimple(Func<object?, object?> conv, Func<object?, object?>? convBack)
    {
        this.conv = conv;
        this.convBack = convBack;
    }

    /// <summary>
    /// create simple one way converter.
    /// </summary>
    /// <param name="conv">converter function. <code>(object? bindingData) => { return viewData; }</code></param>
    /// <returns></returns>
    public static ValueConverterSimple OneWay(Func<object?, object?> conv)
    {
        return new ValueConverterSimple(conv, null);
    }

    /// <summary>
    /// create simple two way converter.
    /// </summary>
    /// <param name="conv">converter function. <code>(object? bindingData) => { return viewData; }</code></param>
    /// <param name="convBack">converter function. <code>(object? viewData) => { return bindingData; }</code></param>
    /// <returns></returns>
    public static ValueConverterSimple TwoWay(Func<object?, object?> conv, Func<object?, object?> convBack)
    {
        return new ValueConverterSimple(conv, convBack);
    }

    /// <inheritdoc cref="IValueConverter.Convert"/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return conv.Invoke(value);
    }

    /// <inheritdoc cref="IValueConverter.ConvertBack"/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (convBack is null)
        {
            throw new NotImplementedException();
        }
        return convBack.Invoke(value);
    }
}
