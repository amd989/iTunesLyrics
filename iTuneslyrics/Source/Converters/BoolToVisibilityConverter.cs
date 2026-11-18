using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace iTuneslyrics.Source.Converters
{
    /// <summary>
    /// true to Visible, false to Collapsed. Pass ConverterParameter="Invert" to flip.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = value is true;
            if (parameter is string p && p.Equals("Invert", StringComparison.OrdinalIgnoreCase))
                flag = !flag;
            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
