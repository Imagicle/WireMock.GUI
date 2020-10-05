using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace WireMock.GUI.Mapping
{
    public class PersistableMappingInfo
    {
        public string Path { get; set; }
        public HttpMethod RequestHttpMethod { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public IDictionary<string, string> Headers { get; set; }
    }
}