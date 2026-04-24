using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectManager.Converters
{
    public sealed class StringEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var left = value?.ToString();
            var right = parameter?.ToString();
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return parameter?.ToString();

            return Binding.DoNothing;
        }
    }
}

