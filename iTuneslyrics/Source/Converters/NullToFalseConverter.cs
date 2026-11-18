using System;
using System.Globalization;
using System.Windows.Data;

namespace iTuneslyrics.Source.Converters
{
    public class NullToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string s ? !string.IsNullOrEmpty(s) : value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
