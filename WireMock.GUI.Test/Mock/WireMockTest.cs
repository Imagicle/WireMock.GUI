using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Serialization;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using NUnit.Framework;
using WireMock.GUI.Mock;
using WireMock.GUI.Model;
using WireMock.GUI.Test.TestUtils;
using WireMock.GUI.Window;

namespace WireMock.GUI.Test.Mock
{
    [TestFixture(HttpMethod.Get)]
    [TestFixture(HttpMethod.Post)]
    [TestFixture(HttpMethod.Put)]
    [TestFixture(HttpMethod.Patch)]
    [TestFixture(HttpMethod.Delete)]
    public class WireMockTest : MockTestBase
    {
        #region Fixture

        private const string WireMockBindUrl = "http://localhost:12345/";
        private readonly HttpMethod _httpMethod;

        #endregion

        public WireMockTest(HttpMethod httpMethod)
        {
            _httpMethod = httpMethod;
        }

        [Test]
        public void ShouldEvaluateAlsoPathsThatContainAQueryString()
        {
            const string path = "a/path/with?aQueryString=true&something=else";
            var mapping = GivenAMapping(new MappingForTest { Path = path });

            var response = MakeHttpRequest("a/path/with?aQueryString=true&something=else", mapping.RequestHttpMethod);

            GetBodyResponse(response).Should().Be(mapping.ResponseBody);
            var errorStatusCode = GetHttpStatusCode(() => MakeHttpRequest("a/path/with?withAnotherQueryString=true", mapping.RequestHttpMethod));
            errorStatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [TestCase(HttpStatusCode.Forbidden)]
        [TestCase(HttpStatusCode.NotFound)]
        [TestCase(HttpStatusCode.InternalServerError)]
        [TestCase(HttpStatusCode.GatewayTimeout)]
        public void ShouldReturnTheExpectedHttpStatusCode(HttpStatusCode httpStatusCode)
        {
            var mapping = GivenAMapping(new MappingForTest { ResponseStatusCode = httpStatusCode });

            var errorStatusCode = GetHttpStatusCode(() => MakeHttpRequest(mapping.Path, mapping.RequestHttpMethod));

            errorStatusCode.Should().Be((int)httpStatusCode);
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("{isBody: true}", "{isBody: true}")]
        [TestCase("b0dy", "b0dy")]
        public void ShouldReturnTheExpectedBody(string body, string expectedBody)
        {
            var mapping = GivenAMapping(new MappingForTest { ResponseBody = body });

            var response = MakeHttpRequest(mapping.Path, mapping.RequestHttpMethod);

            GetBodyResponse(response).Should().Be(expectedBody);
        }

        [Test]
        public void IfResponseBodyContainsTheGuidTag_UpdateMappings_ShouldReplaceItWithARandomGuid()
        {
            var mapping = GivenAMapping(new MappingForTest { ResponseBody = "{\"id\": <guid>}" });

            var response = MakeHttpRequest(mapping.Path, mapping.RequestHttpMethod);

            var id = ReadString<ResponseForTest>(GetBodyResponse(response)).Id;
            id.Should().NotBeEmpty();
            var anotherRequest = MakeHttpRequest(mapping.Path, mapping.RequestHttpMethod);
            var anotherId = ReadString<ResponseForTest>(GetBodyResponse(anotherRequest)).Id;
            id.Should().NotBe(anotherId);
        }

        [TestCase("Cache-Control", "max-age=30")]
        [TestCase("Cache-Control", null)]
        [TestCase("Content-Type", "application/json")]
        public void IfAHeaderIsConfigured_ShouldReturnTheConfiguredResponseHeader(string header, string value)
        {
            var mappingForTest = new MappingForTest();
            mappingForTest.Headers.Add(header, value);
            var mapping = GivenAMapping(mappingForTest);

            var response = MakeHttpRequest(mapping.Path, mapping.RequestHttpMethod);

            response.Headers[header].Should().Be(value);
        }

        [Test]
        public void IfMultipleHeadersAreConfigure_ShouldReturnTheConfiguredResponseHeaders()
        {
            var mappingForTest = new MappingForTest();
            mappingForTest.Headers.Add("Cache-Control", "max-age=30");
            mappingForTest.Headers.Add("Content-Type", "application/json");
            var mapping = GivenAMapping(mappingForTest);

            var response = MakeHttpRequest(mapping.Path, mapping.RequestHttpMethod);

            response.Headers["Cache-Control"].Should().Be("max-age=30");
            response.Headers["Content-Type"].Should().Be("application/json");
        }

        [Test]
        public void WhenANewRequestArrives_ShouldRaiseAnEvent()
        {
            var mapping = GivenAMapping(new MappingForTest());
            using var monitor = MockServer.Monitor();

            MakeHttpRequest(mapping.Path, mapping.RequestHttpMethod);

            monitor.Should()
                .Raise(nameof(MockServer.OnNewRequest))
                .WithArgs(EqualTo(mapping));
        }

        #region Utility Methods

        private MappingInfoViewModel GivenAMapping(MappingForTest mapping)
        {
            return GivenAMappingWith(_httpMethod, mapping);
        }

        private MappingInfoViewModel GivenAMappingWith(HttpMethod requestHttpMethod, MappingForTest mapping)
        {
            var mappingInfoViewModel = new MappingInfoViewModel(A.Fake<IEditResponseWindowFactory>())
            {
                Path = mapping.Path,
                RequestHttpMethod = requestHttpMethod,
                ResponseStatusCode = mapping.ResponseStatusCode,
                ResponseBody = mapping.ResponseBody,
                ResponseHeaders = mapping.Headers
            };

            MockServer.UpdateMappings(new List<MappingInfoViewModel>
            {
                mappingInfoViewModel
            });

            return mappingInfoViewModel;
        }

        private static WebResponse MakeHttpRequest(string path, HttpMethod method)
        {
            var request = WebRequest.Create($"{WireMockBindUrl}{path}");
            request.Method = method.ToString();
            request.ContentLength = 0;

            return request.GetResponse();
        }

        private static string GetBodyResponse(WebResponse response)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var streamReader = new StreamReader(response.GetResponseStream());
            return streamReader.ReadToEnd();
        }

        private static int GetHttpStatusCode(Func<WebResponse> func)
        {
            WebResponse response;
            try
            {
                response = func.Invoke();
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex.Response);
            }

            return GetHttpStatusCode(response);
        }

        private static int GetHttpStatusCode(WebResponse response)
        {
            var webResponse = response as HttpWebResponse;
            // ReSharper disable once PossibleNullReferenceException
            return (int)webResponse.StatusCode;
        }

        private static TBody ReadString<TBody>(string value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;

            return ReadStream<TBody>(stream);
        }

        private Expression<Func<NewRequestEventArgs, bool>> EqualTo(MappingInfoViewModel mapping)
        {
            return args => args.HttpMethod == _httpMethod && args.Path == $"/{mapping.Path}";
        }

        #endregion

        #region Utility Classes

        [DataContract]
        public class ResponseForTest
        {
            [DataMember(Name = "id")]
            public Guid Id { get; set; }
        }

        private class MappingForTest
        {
            public MappingForTest()
            {
                Path = FakerWrapper.Faker.Lorem.Word();
                ResponseStatusCode = HttpStatusCode.OK;
                ResponseBody = FakerWrapper.Faker.Lorem.Sentence();
                Headers = new Dictionary<string, string>();
            }

            public string Path { get; set; }
            public HttpStatusCode ResponseStatusCode { get; set; }
            public string ResponseBody { get; set; }
            public IDictionary<string, string> Headers { get; }
        }

        #endregion
    }
}
