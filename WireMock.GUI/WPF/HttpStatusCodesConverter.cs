using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Data;

namespace WireMock.GUI.WPF
{
    public class HttpStatusCodesConverter : HttStatusCodeConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((HttpStatusCode[])value).Select(ToString).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}