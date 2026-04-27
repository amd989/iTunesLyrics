using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using iTuneslyrics.Source.Models;
using Wpf.Ui.Appearance;

namespace iTuneslyrics.Source.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ResultStatus status)
                return Brushes.Gray;

            var isDark = ApplicationThemeManager.IsMatchedDark();

            return status switch
            {
                ResultStatus.Found => new SolidColorBrush(isDark ? Color.FromRgb(0x30, 0xD1, 0x58) : Color.FromRgb(0x28, 0xA7, 0x45)),
                ResultStatus.Skipped => new SolidColorBrush(isDark ? Color.FromRgb(0xFF, 0xD6, 0x0A) : Color.FromRgb(0xBF, 0x8B, 0x00)),
                ResultStatus.NotFound => new SolidColorBrush(isDark ? Color.FromRgb(0xFF, 0x45, 0x3A) : Color.FromRgb(0xD1, 0x2B, 0x1E)),
                ResultStatus.Error => new SolidColorBrush(isDark ? Color.FromRgb(0xFF, 0x45, 0x3A) : Color.FromRgb(0xD1, 0x2B, 0x1E)),
                ResultStatus.Processing => new SolidColorBrush(isDark ? Color.FromRgb(0x0A, 0x84, 0xFF) : Color.FromRgb(0x00, 0x67, 0xC0)),
                _ => Brushes.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
