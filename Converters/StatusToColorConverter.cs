using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectManager.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6"));

            string status = value.ToString();
            return status switch
            {
                "Новая" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                "В работе" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F39C12")),
                "На проверке" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B59B6")),
                "Завершена" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60")),
                _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6"))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}





