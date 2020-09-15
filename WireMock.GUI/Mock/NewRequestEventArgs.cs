using System;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace WireMock.GUI.Mock
{
    public class NewRequestEventArgs : EventArgs
    {
        public HttpMethod HttpMethod { get; set; }

        public string Path { get; set; }

        public string Body { get; set; }
    }
}