using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using WireMock.GUI.Model;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.GUI.Mock
{
    internal class WireMockWrapper : IMockServer
    {
        private readonly FluentMockServer _mockServer;

        public WireMockWrapper()
        {
            _mockServer = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://localhost:12345/" },
                StartAdminInterface = true
            });
            _mockServer.LogEntriesChanged += OnNewRequestsArrived;
        }

        public event NewRequest OnNewRequest;

        public void UpdateMappings(IEnumerable<MappingInfoViewModel> mappingInfos)
        {
            _mockServer.ResetMappings();
            foreach (var mappingInfo in mappingInfos)
            {
                _mockServer
                    .Given(GetRequest(mappingInfo.Path, mappingInfo.RequestHttpMethod))
                    .RespondWith(GetResponse(mappingInfo.ResponseStatusCode, mappingInfo.ResponseBody, mappingInfo.ResponseHeaders));
            }
        }

        public void Stop()
        {
            _mockServer.Stop();
            Wait.ForCondition(() => !_mockServer.IsStarted);
        }

        #region Utility Methods

        private static IRequestBuilder GetRequest(string pathAndQueryString, HttpMethod httpMethod)
        {
            var (path, queryString) = DividePathAndQueryString(pathAndQueryString);

            return Request.Create()
                .WithPath(path)
                .AddQueryString(queryString)
                .AddHttpMethod(httpMethod);
        }

        private static Tuple<string, string> DividePathAndQueryString(string path)
        {
            var uri = new Uri($"http://justNeededForUsingUri/{path?.TrimStart('/')}");
            return new Tuple<string, string>(uri.AbsolutePath, uri.Query);
        }

        private static IResponseProvider GetResponse(HttpStatusCode statusCode, string body, IDictionary<string, string> headers)
        {
            var response = Response.Create()
                .WithStatusCode(statusCode);

            if (body != null)
            {
                response.WithBody(requestMessage => AdjustBody(body));
            }

            foreach (var header in headers)
            {
                response.WithHeader(header.Key, header.Value);
            }

            return response;
        }

        private static string AdjustBody(string body)
        {
            return Regex.Replace(body, "<guid>", $"\"{Guid.NewGuid()}\"");
        }

        private void OnNewRequestsArrived(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var newItem in e.NewItems)
            {
                var logEntry = (LogEntry)newItem;
                OnNewRequest?.Invoke(new NewRequestEventArgs
                {
                    HttpMethod = ToHttpMethod(logEntry.RequestMessage.Method),
                    Path = logEntry.RequestMessage.Path,
                    Body = logEntry.RequestMessage.Body
                });
            }
        }

        private static HttpMethod ToHttpMethod(string method)
        {
            return method switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "DELETE" => HttpMethod.Delete,
                "PATCH" => HttpMethod.Patch,
                _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
            };
        }

        #endregion
    }

    #region Utility Classes

    public static class WireMockExtensions
    {
        public static IRequestBuilder AddHttpMethod(this IMethodRequestBuilder request, HttpMethod httpMethod)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (httpMethod)
            {
                case HttpMethod.Get:
                    return request.UsingGet();
                case HttpMethod.Post:
                    return request.UsingPost();
                case HttpMethod.Put:
                    return request.UsingPut();
                case HttpMethod.Patch:
                    return request.UsingPatch();
                case HttpMethod.Delete:
                    return request.UsingDelete();
                default:
                    throw new ArgumentOutOfRangeException(nameof(httpMethod), httpMethod, null);
            }
        }

        public static IRequestBuilder AddQueryString(this IRequestBuilder request, string queryString)
        {
            var parsedQueryString = HttpUtility.ParseQueryString(queryString);
            foreach (string parameterName in parsedQueryString)
            {
                request.WithParam(parameterName, new ExactMatcher(parsedQueryString[parameterName]));
            }

            return request;
        }
    }

    #endregion
}
