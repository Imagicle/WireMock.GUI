using System;
using System.Globalization;
using System.Net;
using System.Windows.Data;

namespace WireMock.GUI.WPF
{
    public class HttpStatusCodeConverter : HttStatusCodeConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToString((HttpStatusCode)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse<HttpStatusCode>(((string)value).Split(" - ")[0]);
        }
    }
}