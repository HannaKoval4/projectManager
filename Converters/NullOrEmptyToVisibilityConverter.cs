using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectManager.Converters
{
    public sealed class NullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value?.ToString();
            return string.IsNullOrWhiteSpace(s) || s == "0" ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}

