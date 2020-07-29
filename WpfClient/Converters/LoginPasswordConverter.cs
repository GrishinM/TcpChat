using System;
using System.Globalization;
using System.Security;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfClient.Converters
{
    public class LoginPasswordConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var login = (string) values[0];
            var password = (string) values[1];
            return (login, password);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}