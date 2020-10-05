using System;
using System.Collections.Generic;
using System.Linq;
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
using static WireMock.GUI.Test.TestUtils.FakerWrapper;

namespace WireMock.GUI.Test.Mock
{
    [TestFixture]
    public class WireMockWrapperTest : MockTestBase
    {
        #region Fixture

        private const string WireMockDefaultBindUrl = "http://localhost:12345/";

        #endregion

        #region Constructor

        [Test]
        public void Constructor_ShouldStartWireMock()
        {
            WireMockShouldBeStarted();
        }

        #endregion

        #region UpdateMappings

        [TestCase("aPath", "/aPath")]
        [TestCase("/anotherPath", "/anotherPath")]
        public void IfAndOnlyIfInitialSlashIsNotPresent_UpdateMappings_ShouldAddIt(string path, string expectedPath)
        {
            var mappingInfoViewModels = new List<MappingInfoViewModel>
            {
                new MappingInfoViewModel(A.Fake<IEditResponseWindowFactory>()) { Path = path }
            };

            MockServer.UpdateMappings(mappingInfoViewModels);

            var wireMockMappings = GetWireMockMappings();
            wireMockMappings.Single().Request.Path.Matchers.Single().Pattern.Should().Be(expectedPath);
        }

        [TestCase(HttpMethod.Head)]
        [TestCase(HttpMethod.Trace)]
        [TestCase(HttpMethod.Connect)]
        [TestCase(HttpMethod.Options)]
        [TestCase(HttpMethod.Custom)]
        [TestCase(HttpMethod.None)]
        public void IfGivenHttpMethodIsInvalid_UpdateMappings_ShouldThrowAnException(HttpMethod invalidHttpMethod)
        {
            var mappingInfoViewModels = new List<MappingInfoViewModel>
            {
                new MappingInfoViewModel(A.Fake<IEditResponseWindowFactory>()) { RequestHttpMethod = invalidHttpMethod }
            };

            MockServer.Invoking(m => m.UpdateMappings(mappingInfoViewModels)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void UpdateMappings_ShouldUpdateWireMockMappings()
        {
            var mappingInfoViewModels = MappingInfoViewModelTestUtils.SomeMappings();

            MockServer.UpdateMappings(mappingInfoViewModels);

            WireMockMappingsShouldBeConfiguredWith(mappingInfoViewModels);
        }

        [Test]
        public void UpdateMappings_ShouldClearPreviousConfiguration()
        {
            var mappingInfoViewModels = MappingInfoViewModelTestUtils.SomeMappings();
            MockServer.UpdateMappings(MappingInfoViewModelTestUtils.SomeMappings());

            MockServer.UpdateMappings(mappingInfoViewModels);

            WireMockMappingsShouldBeConfiguredWith(mappingInfoViewModels);
        }

        #endregion

        #region Url

        [Test]
        public void Url_ShouldHaveDefaultValue()
        {
            MockServer.Url.Should().Be(WireMockDefaultBindUrl);
        }

        [Test]
        public void IfServiceIsStopped_Url_ShouldBeEditable()
        {
            MockServer.Stop();
            var expectedUrl = Faker.Internet.Url();

            MockServer.Url = expectedUrl;

            MockServer.Url.Should().Be(expectedUrl);
        }

        [Test]
        public void IfServiceIsStarted_Url_ShouldNotBeEditable()
        {
            var expectedUrl = Faker.Internet.Url();

            this.Invoking(t => MockServer.Url = expectedUrl)
                .Should().ThrowExactly<InvalidOperationException>()
                .And.Message.Should().Be("Cannot change the mock server URL if the server is running!");
            MockServer.Url.Should().NotBe(expectedUrl);
        }

        #endregion

        #region Start

        [Test]
        public void Start_ShouldStartWireMock()
        {
            MockServer.Stop();

            MockServer.Start();

            WireMockShouldBeStarted();
        }

        [Test]
        public void Start_ShouldRaiseOnServerStatusChangeEvent()
        {
            MockServer.Stop();
            using var monitor = MockServer.Monitor();
            MockServer.Start();

            monitor.Should()
                .Raise(nameof(MockServer.OnServerStatusChange))
                .WithArgs(EqualTo(true));
        }

        #endregion

        #region Stop

        [Test]
        public void Stop_ShouldStopWireMock()
        {
            MockServer.Stop();

            WireMockShouldBeStopped();
        }

        [Test]
        public void Stop_ShouldRaiseOnServerStatusChangeEvent()
        {
            using var monitor = MockServer.Monitor();
            MockServer.Stop();

            monitor.Should()
                .Raise(nameof(MockServer.OnServerStatusChange))
                .WithArgs(EqualTo(false));
        }

        #endregion

        #region Utility Methods

        private static void WireMockShouldBeStarted()
        {
            IsWireMockRunning().Should().BeTrue();
        }

        private static void WireMockShouldBeStopped()
        {
            IsWireMockRunning().Should().BeFalse();
        }

        private static bool IsWireMockRunning()
        {
            try
            {
                WebRequest.Create($"{WireMockDefaultBindUrl}__admin/mappings").GetResponse();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static void WireMockMappingsShouldBeConfiguredWith(ICollection<MappingInfoViewModel> mappingInfoViewModels)
        {
            var wireMockMappings = GetWireMockMappings();
            ShouldBeEquivalent(wireMockMappings, mappingInfoViewModels);
        }

        private static void ShouldBeEquivalent(IList<Mapping> wireMockMappings, ICollection<MappingInfoViewModel> mappingInfoViewModels)
        {
            wireMockMappings.Should().HaveCount(mappingInfoViewModels.Count);
            foreach (var wireMockMapping in wireMockMappings)
            {
                mappingInfoViewModels.Any(mvm => ShouldBeEquivalent(wireMockMapping, mvm)).Should().BeTrue();
            }
        }

        private static bool ShouldBeEquivalent(Mapping wireMockMapping, MappingInfoViewModel mappingInfoViewModel)
        {
            return wireMockMapping.Request.Path.Matchers.First().Pattern.Equals($"/{mappingInfoViewModel.Path}") &&
                   wireMockMapping.Request.Methods.First().Equals(mappingInfoViewModel.RequestHttpMethod.ToString(), StringComparison.InvariantCultureIgnoreCase) &&
                   wireMockMapping.Response.StatusCode.Equals(mappingInfoViewModel.ResponseStatusCode) &&
                   ShouldBeEquivalent(wireMockMapping.Response.Headers, mappingInfoViewModel.ResponseHeaders);
        }

        private static bool ShouldBeEquivalent(Headers wireMockHeaders, IDictionary<string, string> mappingInfoViewModelHeaders)
        {
            return wireMockHeaders.ContentType == mappingInfoViewModelHeaders["Content-Type"] &&
                   wireMockHeaders.CacheControl == mappingInfoViewModelHeaders["Cache-Control"];
        }

        private static IList<Mapping> GetWireMockMappings()
        {
            var webResponse = WebRequest.Create($"{WireMockDefaultBindUrl}__admin/mappings").GetResponse();
            return ReadStream<Mapping[]>(webResponse.GetResponseStream());
        }

        private static Expression<Func<ServerStatusChangeEventArgs, bool>> EqualTo(bool isStarted)
        {
            return args => args.IsStarted == isStarted;
        }

        #endregion

        #region Utility Classes

        [DataContract]
        public class Mapping
        {
            [DataMember(Name = "Guid")]
            public Guid Id { get; set; }

            [DataMember(Name = "Request")]
            public Request Request { get; set; }

            [DataMember(Name = "Response")]
            public Response Response { get; set; }
        }

        [DataContract]
        public class Request
        {
            [DataMember(Name = "Path")]
            public Path Path { get; set; }

            [DataMember(Name = "Methods")]
            public IEnumerable<string> Methods { get; set; }
        }

        [DataContract]
        public class Path
        {
            [DataMember(Name = "Matchers")]
            public IEnumerable<Matcher> Matchers { get; set; }
        }

        [DataContract]
        public class Matcher
        {
            [DataMember(Name = "Name")]
            public string Name;

            [DataMember(Name = "Pattern")]
            public string Pattern;

            [DataMember(Name = "IgnoreCase")]
            public bool IgnoreCase;
        }

        [DataContract]
        public class Response
        {
            [DataMember(Name = "StatusCode")]
            public HttpStatusCode StatusCode { get; set; }

            [DataMember(Name = "Headers")]
            public Headers Headers { get; set; }
        }

        [DataContract]
        public class Headers
        {
            [DataMember(Name = "Content-Type")]
            public string ContentType { get; set; }

            [DataMember(Name = "Cache-Control")]
            public string CacheControl { get; set; }
        }

        #endregion
    }
}
