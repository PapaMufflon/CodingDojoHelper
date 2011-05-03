using System.Windows.Data;
using System.Windows.Forms;

namespace CodingDojoHelper.Converter
{
    class KeyToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var key = (Keys) value;
            return "<"+key+">";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
