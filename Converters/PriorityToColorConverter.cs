using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectManager.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6"));

            string priority = value.ToString();
            return priority switch
            {
                "Низкий" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                "Средний" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F39C12")),
                "Высокий" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E67E22")),
                "Критический" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")),
                _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6"))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}





