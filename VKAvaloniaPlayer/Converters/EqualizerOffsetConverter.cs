using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace VKAvaloniaPlayer.Converters;

public class EqualizerOffsetConverter:IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double.NaN) return value;
        if (value is double)
            return - (int)((double)value/2);
        else if (value is int)
        {
            return -(int)((int)value / 2);
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}