using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectManager.Converters
{
    /// <summary>Срочные/близкие дедлайны — акцентный красный, иначе приглушённый серый (#7F8C8D).</summary>
    public class DeadlineToForegroundBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush UrgentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
        private static readonly SolidColorBrush NormalBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == System.Windows.DependencyProperty.UnsetValue)
                return NormalBrush;
            if (value is DateTime dt)
            {
                return dt.Date <= DateTime.Today.AddDays(21) ? UrgentBrush : NormalBrush;
            }
            return NormalBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
