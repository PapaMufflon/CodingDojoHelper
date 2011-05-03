using System;
using System.Windows.Data;

namespace CodingDojoHelper.Converter
{
    class TimeSpanToHoursMinutesConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var timeSpan = (TimeSpan) value;
            return string.Format("{0}:{1:00}", timeSpan.Hours, timeSpan.Minutes);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
