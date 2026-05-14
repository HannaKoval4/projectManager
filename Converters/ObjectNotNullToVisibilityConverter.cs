using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectManager.Converters
{
    public sealed class ObjectNotNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase);
            bool has = value != null;
            if (invert) has = !has;
            return has ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
