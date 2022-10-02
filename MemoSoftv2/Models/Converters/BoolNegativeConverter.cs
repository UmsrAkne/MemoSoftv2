using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoSoftv2.Models.Converters
{
    public class BoolNegativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}