using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoSoftv2.Models.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime?)value)?.ToString("yy/MM/dd HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}