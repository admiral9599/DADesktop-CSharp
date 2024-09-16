using System;
using System.Globalization;
using System.Windows.Data;

namespace DriveAdviser.UI.Drives
{
    class HealthToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var health = (int)value;

            return health < 100 ? "HeartBreak" : "Heart";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
