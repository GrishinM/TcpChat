using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ChatLibrary;

namespace WpfClient.Converters
{
    public class MessageColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sender = (ClientInfo) values[0];
            var owner = (ClientInfo) values[1];
            const int a = 240;
            return sender.Login == owner.Login ? Brushes.LightSkyBlue : new SolidColorBrush(Color.FromRgb(a,a,a));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}