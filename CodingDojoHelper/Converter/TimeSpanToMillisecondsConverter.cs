using System;
using System.Windows.Data;

namespace CodingDojoHelper.Converter
{
    public class TimeSpanToMillisecondsConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "." + ((TimeSpan) value).Milliseconds;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}