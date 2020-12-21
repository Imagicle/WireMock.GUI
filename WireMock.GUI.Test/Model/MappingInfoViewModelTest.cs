using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using NUnit.Framework;
using WireMock.GUI.Model;
using WireMock.GUI.Window;
using static WireMock.GUI.Test.TestUtils.FakerWrapper;

namespace WireMock.GUI.Test.Model
{
    [TestFixture]
    public class MappingInfoViewModelTest
    {
        #region Fixture

        private IEditResponseWindowFactory _editResponseWindowFactory;
        private MappingInfoViewModel _mappingInfoViewModel;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _editResponseWindowFactory = A.Fake<IEditResponseWindowFactory>();
            _mappingInfoViewModel = new MappingInfoViewModel(_editResponseWindowFactory);
        }

        #endregion

        #region Path

        [Test]
        public void WhenPathIsModified_Path_ShouldRaiseAnEvent()
        {
            var expectedPath = Faker.System.DirectoryPath();
            using var monitor = _mappingInfoViewModel.Monitor();

            _mappingInfoViewModel.Path = expectedPath;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.Path);
        }

        [Test]
        public void Path_ShouldBeEditable()
        {
            var expectedPath = Faker.System.DirectoryPath();

            _mappingInfoViewModel.Path = expectedPath;

            _mappingInfoViewModel.Path.Should().Be(expectedPath);
        }

        #endregion

        #region RequestHttpMethod

        [Test]
        public void WhenRequestHttpMethodIsModified_RequestHttpMethod_ShouldRaiseAnEvent()
        {
            var expectedRequestHttpMethod = Faker.PickRandom<HttpMethod>();
            using var monitor = _mappingInfoViewModel.Monitor();

            _mappingInfoViewModel.RequestHttpMethod = expectedRequestHttpMethod;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.RequestHttpMethod);
        }

        [Test]
        public void RequestHttpMethod_ShouldBeEditable()
        {
            var expectedRequestHttpMethod = Faker.PickRandom<HttpMethod>();

            _mappingInfoViewModel.RequestHttpMethod = expectedRequestHttpMethod;

            _mappingInfoViewModel.RequestHttpMethod.Should().Be(expectedRequestHttpMethod);
        }

        #endregion

        #region HttpMethods

        [Test]
        public void HttpMethods_ShouldReturnExpectedValues()
        {
            var expectedHttpMethods = new List<HttpMethod>
            {
                HttpMethod.Get,
                HttpMethod.Post,
                HttpMethod.Put,
                HttpMethod.Patch,
                HttpMethod.Delete
            };

            var httpMethods = MappingInfoViewModel.HttpMethods;

            httpMethods.Should().BeEquivalentTo(expectedHttpMethods);
        }

        #endregion

        #region ResponseStatusCode

        [Test]
        public void WhenResponseStatusCodeIsModified_ResponseStatusCode_ShouldRaiseAnEvent()
        {
            var expectedResponseStatusCode = Faker.PickRandom<HttpStatusCode>();
            using var monitor = _mappingInfoViewModel.Monitor();

            _mappingInfoViewModel.ResponseStatusCode = expectedResponseStatusCode;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.ResponseStatusCode);
        }

        [Test]
        public void ResponseStatusCode_ShouldBeEditable()
        {
            var expectedResponseStatusCode = Faker.PickRandom<HttpStatusCode>();

            _mappingInfoViewModel.ResponseStatusCode = expectedResponseStatusCode;

            _mappingInfoViewModel.ResponseStatusCode.Should().Be(expectedResponseStatusCode);
        }

        #endregion

        #region HttpStatusCodes

        [Test]
        public void HttpStatusCodes_ShouldReturnExpectedValues()
        {
            var expectedHttpStatusCodes = Enum.GetValues(typeof(HttpStatusCode)).Cast<HttpStatusCode>();

            var httpStatusCodes = MappingInfoViewModel.HttpStatusCodes;

            httpStatusCodes.Should().BeEquivalentTo(expectedHttpStatusCodes);
        }

        #endregion

        #region ResponseBody

        [Test]
        public void WhenResponseBodyIsModified_ResponseBody_ShouldRaiseAnEvent()
        {
            var expectedResponseBody = Faker.Lorem.Sentence();
            using var monitor = _mappingInfoViewModel.Monitor();

            _mappingInfoViewModel.ResponseBody = expectedResponseBody;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.MinifiedResponseBody);
        }

        [Test]
        public void ResponseBody_ShouldBeEditable()
        {
            var expectedResponseBody = Faker.Lorem.Sentence();

            _mappingInfoViewModel.ResponseBody = expectedResponseBody;

            _mappingInfoViewModel.ResponseBody.Should().Be(expectedResponseBody);
        }

        #endregion

        #region MinifiedResponseBody

        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase("{\n\t\"id\": \"anId\"\n}", "{\"id\":\"anId\"}")]
        [TestCase("Invalid json", "Invalid json")]
        [TestCase("{\"boh\": \"\t json \r\n\"}", "{\"boh\":\"\\t json \\r\\n\"}")]
        public void MinifiedResponseBody_ShouldReturnTheMinifiedVersionOfResponseBodyProperty(string responseBody, string expectedMinifiedResponseBody)
        {
            _mappingInfoViewModel.ResponseBody = responseBody;

            var minifiedResponseBody = _mappingInfoViewModel.MinifiedResponseBody;

            minifiedResponseBody.Should().Be(expectedMinifiedResponseBody);
        }

        #endregion

        #region Headers

        [Test]
        public void Headers_ShouldByDefaultAnEmptyList()
        {
            _mappingInfoViewModel.ResponseHeaders.Should().BeEmpty();
        }

        [Test]
        public void Headers_ShouldBeEditable()
        {
            var expectedResponseCacheControlMaxAge = new Dictionary<string, string>
            {
                {Faker.Lorem.Word(), Faker.Lorem.Word()},
                {Faker.Lorem.Word(), Faker.Lorem.Word()}
            };

            _mappingInfoViewModel.ResponseHeaders = expectedResponseCacheControlMaxAge;

            _mappingInfoViewModel.ResponseHeaders.Should().BeEquivalentTo(expectedResponseCacheControlMaxAge);
        }

        #endregion

        #region EditResponseCommand

        [Test]
        public void WhenEditResponseWindowIsOpened_EditResponseCommand_ShouldOpenItWithTheActualResponseBodyValue()
        {
            var expectedResponseBody = Faker.Lorem.Sentence();
            _mappingInfoViewModel.ResponseBody = expectedResponseBody;
            var editResponseWindow = GivenAEditResponseWindow(true, expectedResponseBody);

            ExecuteEditResponseCommand();

            A.CallToSet(() => editResponseWindow.Body).WhenArgumentsMatch(args => args.Get<string>(0) == expectedResponseBody).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void WhenEditResponseWindowIsClosedWithOkButton_EditResponseCommand_ShouldModifyResponseBodyWithTheValueInsertedInTheEditResponseWindow()
        {
            var expectedResponseBody = Faker.Lorem.Sentence();
            GivenAEditResponseWindow(true, expectedResponseBody);

            ExecuteEditResponseCommand();

            _mappingInfoViewModel.ResponseBody.Should().Be(expectedResponseBody);
        }

        [Test]
        public void WhenEditResponseWindowIsClosedWithExitButton_EditResponseCommand_ShouldNotModifyTheResponseBody()
        {
            var expectedResponseBody = Faker.Lorem.Sentence();
            _mappingInfoViewModel.ResponseBody = expectedResponseBody;
            GivenAEditResponseWindow(false, Faker.Lorem.Sentence());

            ExecuteEditResponseCommand();

            _mappingInfoViewModel.ResponseBody.Should().Be(expectedResponseBody);
        }

        [Test]
        public void WhenEditResponseWindowIsOpened_EditResponseCommand_ShouldOpenItWithTheActualResponseHeadersValue()
        {
            var expectedResponseHeaders = GivenSomeHeaders();
            _mappingInfoViewModel.ResponseHeaders = expectedResponseHeaders;
            var editResponseWindow = GivenAEditResponseWindow(true, expectedResponseHeaders);

            ExecuteEditResponseCommand();

            foreach (var (key, value) in expectedResponseHeaders.Reverse())
            {
                A.CallTo(() => editResponseWindow.AddHeader(key, value)).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void WhenEditResponseWindowIsClosedWithOkButton_EditResponseCommand_ShouldModifyResponseHeadersWithTheValueInsertedInTheEditResponseWindow()
        {
            var expectedResponseHeaders = GivenSomeHeaders();
            GivenAEditResponseWindow(true, expectedResponseHeaders);

            ExecuteEditResponseCommand();

            _mappingInfoViewModel.ResponseHeaders.Should().BeSameAs(expectedResponseHeaders);
        }

        [Test]
        public void WhenEditResponseWindowIsClosedWithExitButton_EditResponseCommand_ShouldNotModifyTheResponseHeaders()
        {
            var expectedResponseHeaders = GivenSomeHeaders();
            _mappingInfoViewModel.ResponseHeaders = expectedResponseHeaders;
            GivenAEditResponseWindow(false, GivenSomeHeaders());

            ExecuteEditResponseCommand();

            _mappingInfoViewModel.ResponseHeaders.Should().BeSameAs(expectedResponseHeaders);
        }

        #endregion

        #region DeleteMappingCommand

        [Test]
        public void IfThereAreNoSubscribersForTheEventOnDeleteMapping_DeleteMappingCommand_ShouldNotThrow()
        {
            this.Invoking(t => t.ExecuteDeleteMappingCommand()).Should().NotThrow();
        }

        [Test]
        public void DeleteMappingCommand_ShouldThrowOnDeleteEvent()
        {
            using var monitor = _mappingInfoViewModel.Monitor();

            ExecuteDeleteMappingCommand();

            monitor.Should().Raise(nameof(_mappingInfoViewModel.OnDeleteMapping));
        }

        #endregion

        #region Utility Methods

        private IEditResponseWindow GivenAEditResponseWindow(bool showDialogResult, string expectedResponseBody)
        {
            var editResponseWindow = A.Fake<IEditResponseWindow>();
            A.CallTo(() => editResponseWindow.ShowDialog()).Returns(showDialogResult);
            A.CallTo(() => editResponseWindow.Body).Returns(expectedResponseBody);
            A.CallTo(() => _editResponseWindowFactory.Create()).Returns(editResponseWindow);

            return editResponseWindow;
        }

        private IEditResponseWindow GivenAEditResponseWindow(bool showDialogResult, IDictionary<string, string> expectedResponseHeaders)
        {
            var editResponseWindow = A.Fake<IEditResponseWindow>();
            A.CallTo(() => editResponseWindow.ShowDialog()).Returns(showDialogResult);
            A.CallTo(() => editResponseWindow.Headers).Returns(expectedResponseHeaders);
            A.CallTo(() => _editResponseWindowFactory.Create()).Returns(editResponseWindow);

            return editResponseWindow;
        }

        private static IDictionary<string, string> GivenSomeHeaders()
        {
            return new Dictionary<string, string>
            {
                { Faker.Lorem.Word(), Faker.Lorem.Word() },
                { Faker.Lorem.Word(), Faker.Lorem.Word() },
                { Faker.Lorem.Word(), Faker.Lorem.Word() }
            };
        }

        private void ExecuteEditResponseCommand()
        {
            _mappingInfoViewModel.EditResponseCommand.Execute(null);
        }

        private void ExecuteDeleteMappingCommand()
        {
            _mappingInfoViewModel.DeleteMappingCommand.Execute(null);
        }

        #endregion
    }
}
