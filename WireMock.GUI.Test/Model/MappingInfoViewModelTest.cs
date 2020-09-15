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

        private ITextAreaWindowFactory _textAreaWindowFactory;
        private MappingInfoViewModel _mappingInfoViewModel;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _textAreaWindowFactory = A.Fake<ITextAreaWindowFactory>();
            _mappingInfoViewModel = new MappingInfoViewModel(_textAreaWindowFactory);
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

        #region ResponseCacheControlMaxAge

        [Test]
        public void WhenResponseCacheControlMaxAgeIsModified_ResponseCacheControlMaxAge_ShouldRaiseAnEvent()
        {
            var expectedResponseCacheControlMaxAge = Faker.Random.Number().ToString();
            using var monitor = _mappingInfoViewModel.Monitor();
            
            _mappingInfoViewModel.ResponseCacheControlMaxAge = expectedResponseCacheControlMaxAge;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.ResponseCacheControlMaxAge);
        }

        [Test]
        public void ResponseCacheControlMaxAge_ShouldBeEditable()
        {
            var expectedResponseCacheControlMaxAge = Faker.Random.Number().ToString();

            _mappingInfoViewModel.ResponseCacheControlMaxAge = expectedResponseCacheControlMaxAge;

            _mappingInfoViewModel.ResponseCacheControlMaxAge.Should().Be(expectedResponseCacheControlMaxAge);
        }

        #endregion

        #region EditBodyCommand

        [Test]
        public void WhenTextAreaWindowIsOpened_EditBodyCommand_ShouldOpenItWithTheActualResponseBodyValue()
        {
            var expectedResponseBody = Faker.Lorem.Sentence();
            _mappingInfoViewModel.ResponseBody = expectedResponseBody;
            var textAreaWindow = GivenATextAreaWindow(true, expectedResponseBody);

            ExecuteEditBodyCommand();

            A.CallToSet(() => textAreaWindow.InputValue).WhenArgumentsMatch(args => args.Get<string>(0) == expectedResponseBody).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void WhenTextAreaWindowIsClosedWithOkButton_EditBodyCommand_ShouldModifyResponseBodyWithTheValueInsertedInTheTextAreaWindow()
        {
            var expectedResponseBody = Faker.Lorem.Sentence();
            GivenATextAreaWindow(true, expectedResponseBody);

            ExecuteEditBodyCommand();

            _mappingInfoViewModel.ResponseBody.Should().Be(expectedResponseBody);
        }

        [Test]
        public void WhenTextAreaWindowIsClosedWithExitButton_EditBodyCommand_ShouldNotModifyTheResponseBody()
        {
            var expectedResponseBody = Faker.Lorem.Sentence();
            _mappingInfoViewModel.ResponseBody = expectedResponseBody;
            GivenATextAreaWindow(false, Faker.Lorem.Sentence());

            ExecuteEditBodyCommand();

            _mappingInfoViewModel.ResponseBody.Should().Be(expectedResponseBody);
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

        private ITextAreaWindow GivenATextAreaWindow(bool showDialogResult, string expectedResponseBody)
        {
            var textAreaWindow = A.Fake<ITextAreaWindow>();
            A.CallTo(() => textAreaWindow.ShowDialog()).Returns(showDialogResult);
            A.CallTo(() => textAreaWindow.InputValue).Returns(expectedResponseBody);
            A.CallTo(() => _textAreaWindowFactory.Create()).Returns(textAreaWindow);

            return textAreaWindow;
        }

        private void ExecuteEditBodyCommand()
        {
            _mappingInfoViewModel.EditBodyCommand.Execute(null);
        }

        private void ExecuteDeleteMappingCommand()
        {
            _mappingInfoViewModel.DeleteMappingCommand.Execute(null);
        }

        #endregion
    }
}
