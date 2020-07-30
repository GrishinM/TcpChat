using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ChatLibrary;

namespace WpfClient.Converters
{
    public class MessageAlignmentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sender = (ClientInfo) values[0];
            var owner = (ClientInfo) values[1];
            return sender.Login == owner.Login ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}