namespace MemoSoftv2.Models.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mc = default(System.Windows.Media.Color);
            var dc = System.Drawing.Color.FromArgb((int)value);
            mc.A = dc.A;
            mc.R = dc.R;
            mc.G = dc.G;
            mc.B = dc.B;

            return new System.Windows.Media.SolidColorBrush(mc);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
