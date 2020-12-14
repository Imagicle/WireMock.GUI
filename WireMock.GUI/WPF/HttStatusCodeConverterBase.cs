using System.Net;

namespace WireMock.GUI.WPF
{
    public abstract class HttStatusCodeConverterBase
    {
        protected static string ToString(HttpStatusCode statusCode)
        {
            return $"{(int)statusCode} - {statusCode}";
        }
    }
}