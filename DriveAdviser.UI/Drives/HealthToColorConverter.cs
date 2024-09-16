using System;
using System.Globalization;
using System.Windows.Data;

namespace DriveAdviser.UI.Drives
{
    public class HealthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var health = (int) value;
            if (health <100 && health >70)
            {
                return "Gold";
            }
            else if (health <= 70)
            {
                return "DarkRed";
            }
            else
            {
                return "DarkGreen";
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
