using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace WireMock.GUI.Mapping
{
    internal class PersistableMappingInfo
    {
        public string Path { get; set; }
        public HttpMethod RequestHttpMethod { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public string ResponseCacheControlMaxAge { get; set; }
    }
}