using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AliNedvizhimostApp.Converters
{
    public class SenderToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSender)
            {
                return isSender ? new SolidColorBrush(Color.FromRgb(220, 248, 198)) : new SolidColorBrush(Color.FromRgb(240, 240, 240)); // Light green for sender, light gray for receiver
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
