using Avalonia.Data.Converters;

using System;
using System.Globalization;

namespace VKAvaloniaPlayer.Converters
{
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var time = new TimeSpan(0, 0, (int)value);
                return time.Hours > 0 ? time.ToString(@"hh\:mm\:ss") : time.ToString(@"m\:ss");
            }
            catch (Exception EX)
            {
                return "00:00";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}