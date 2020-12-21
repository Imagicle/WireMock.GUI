using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WireMock.GUI.Model;
using WireMock.GUI.Test.TestUtils;

namespace WireMock.GUI.Test.Model
{
    [TestFixture]
    public class EditResponseViewModelTest
    {
        #region Fixture

        private EditResponseViewModel _editResponseViewModel;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _editResponseViewModel = new EditResponseViewModel();
        }

        #endregion

        #region Body

        [Test]
        public void WhenBodyIsModified_Body_ShouldRaiseAnEvent()
        {
            var expectedBody = FakerWrapper.Faker.Lorem.Paragraph();
            using var monitor = _editResponseViewModel.Monitor();

            _editResponseViewModel.Body = expectedBody;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.Body);
        }

        [Test]
        public void Body_ShouldBeEditable()
        {
            var expectedBody = FakerWrapper.Faker.Lorem.Paragraph();

            _editResponseViewModel.Body = expectedBody;

            _editResponseViewModel.Body.Should().Be(expectedBody);
        }

        #endregion

        #region HeadersDictionary

        [Test]
        public void WhenModelIsInstantiated_HeadersDictionary_ShouldReturnAnEmptyList()
        {
            _editResponseViewModel = new EditResponseViewModel();

            _editResponseViewModel.HeadersDictionary.Should().BeEmpty();
        }

        #endregion

        #region Headers

        [Test]
        public void WhenModelIsInstantiated_Headers_ShouldReturnAnEmptyList()
        {
            _editResponseViewModel = new EditResponseViewModel();

            _editResponseViewModel.Headers.Should().BeEmpty();
        }

        #endregion

        #region IsInputValid

        [Test]
        public void IfHeadersIsEmpty_IsInputValid_ShouldReturnTrue()
        {
            var isInputValid = _editResponseViewModel.IsInputValid;

            isInputValid.Should().BeTrue();
        }

        [Test]
        public void IfAddedHeadersAreValid_IsInputValid_ShouldReturnTrue()
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), null);
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), string.Empty);
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), "  \t ");
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());

            var isInputValid = _editResponseViewModel.IsInputValid;

            isInputValid.Should().BeTrue();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.IsInputValid);
        }

        [Test]
        public void IfHeaderBecomesValid_IsInputValid_ShouldReturnTrue()
        {
            _editResponseViewModel.AddHeader(null, null);
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.Headers[0].Key = FakerWrapper.Faker.Lorem.Paragraph();

            var isInputValid = _editResponseViewModel.IsInputValid;

            isInputValid.Should().BeTrue();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.IsInputValid);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase(" \t ")]
        public void IfAddedHeaderHasInvalidKey_IsInputValid_ShouldReturnFalse(string invalidHeaderKey)
        {
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.AddHeader(invalidHeaderKey, FakerWrapper.Faker.Lorem.Paragraph());

            var isInputValid = _editResponseViewModel.IsInputValid;

            isInputValid.Should().BeFalse();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.IsInputValid);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase(" \t ")]
        public void IfHeaderBecomesInvalid_IsInputValid_ShouldReturnFalse(string invalidHeaderKey)
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), null);
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.Headers[0].Key = invalidHeaderKey;

            var isInputValid = _editResponseViewModel.IsInputValid;

            isInputValid.Should().BeFalse();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.IsInputValid);
        }

        [Test]
        public void IfAddedHeaderKeyIsAlreadyUsedByAnotherHeader_IsInputValid_ShouldReturnFalse()
        {
            var duplicatedHeaderKey = FakerWrapper.Faker.Lorem.Word();
            _editResponseViewModel.AddHeader(duplicatedHeaderKey, FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.AddHeader(duplicatedHeaderKey, FakerWrapper.Faker.Lorem.Paragraph());

            var isInputValid = _editResponseViewModel.IsInputValid;

            isInputValid.Should().BeFalse();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.IsInputValid);
        }

        [Test]
        public void IfHeadersBecomeInvalidBecauseTheSameKeyIsUsedMultipleTimes_IsInputValid_ShouldReturnFalse()
        {
            var duplicatedHeaderKey = FakerWrapper.Faker.Lorem.Word();
            _editResponseViewModel.AddHeader(duplicatedHeaderKey, FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.Headers[2].Key = duplicatedHeaderKey;

            var isInputValid = _editResponseViewModel.IsInputValid;

            isInputValid.Should().BeFalse();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.IsInputValid);
        }

        [Test]
        public void IfInvalidHeaderIsRemoved_IsInputValid_ShouldReturnTrue()
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Word(), FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(null, FakerWrapper.Faker.Lorem.Paragraph());
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.Headers.RemoveAt(1);

            var isInputValid = _editResponseViewModel.IsInputValid;

            isInputValid.Should().BeTrue();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.IsInputValid);
        }

        [Test]
        public void IfHeaderIsRemoved_IsInputValid_ShouldNotRaiseTheEventPropertyChange()
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Word(), FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Word(), FakerWrapper.Faker.Lorem.Paragraph());
            var headerToBeRemoved = _editResponseViewModel.Headers[1];
            _editResponseViewModel.Headers.Remove(headerToBeRemoved);
            using var monitor = _editResponseViewModel.Monitor();
            headerToBeRemoved.Key = FakerWrapper.Faker.Lorem.Word();

            var unused = _editResponseViewModel.IsInputValid;

            monitor.Should().NotRaisePropertyChangeFor(viewModel => viewModel.IsInputValid);
        }

        #endregion

        #region InputErrorMessage

        [Test]
        public void InputErrorMessage_ShouldBeNullByDefault()
        {
            var inputErrorMessage = _editResponseViewModel.InputErrorMessage;

            inputErrorMessage.Should().BeNull();
        }

        [Test]
        public void IfAddedHeadersAreValid_InputErrorMessage_ShouldBeNull()
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), null);
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), string.Empty);
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), "  \t ");
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());

            var inputErrorMessage = _editResponseViewModel.InputErrorMessage;

            inputErrorMessage.Should().BeNull();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.InputErrorMessage);
        }

        [Test]
        public void IfHeaderBecomesValid_InputErrorMessage_ShouldBeNull()
        {
            _editResponseViewModel.AddHeader(null, null);
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.Headers[0].Key = FakerWrapper.Faker.Lorem.Paragraph();

            var inputErrorMessage = _editResponseViewModel.InputErrorMessage;

            inputErrorMessage.Should().BeNull();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.InputErrorMessage);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase(" \t ")]
        public void IfAddedHeaderHasInvalidKey_InputErrorMessage_ShouldReturnSpecificErrorMessage(string invalidHeaderKey)
        {
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.AddHeader(invalidHeaderKey, FakerWrapper.Faker.Lorem.Paragraph());

            var inputErrorMessage = _editResponseViewModel.InputErrorMessage;
            inputErrorMessage.Should().Be("Null or empty header keys are not allowed");
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.InputErrorMessage);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase(" \t ")]
        public void IfHeaderBecomesInvalid_InputErrorMessage_ShouldReturnSpecificErrorMessage(string invalidHeaderKey)
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), null);
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.Headers[0].Key = invalidHeaderKey;

            var inputErrorMessage = _editResponseViewModel.InputErrorMessage;

            inputErrorMessage.Should().Be("Null or empty header keys are not allowed");
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.InputErrorMessage);
        }

        [Test]
        public void IfAddedHeaderKeyIsAlreadyUsedByAnotherHeader_InputErrorMessage_ShouldReturnSpecificErrorMessage()
        {
            var duplicatedHeaderKey = FakerWrapper.Faker.Lorem.Word();
            _editResponseViewModel.AddHeader(duplicatedHeaderKey, FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.AddHeader(duplicatedHeaderKey, FakerWrapper.Faker.Lorem.Paragraph());

            var inputErrorMessage = _editResponseViewModel.InputErrorMessage;

            inputErrorMessage.Should().Be("The same key is specified multiple times");
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.InputErrorMessage);
        }

        [Test]
        public void IfHeadersBecomeInvalidBecauseTheSameKeyIsUsedMultipleTimes_InputErrorMessage_ShouldReturnSpecificErrorMessage()
        {
            var duplicatedHeaderKey = FakerWrapper.Faker.Lorem.Word();
            _editResponseViewModel.AddHeader(duplicatedHeaderKey, FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.Headers[2].Key = duplicatedHeaderKey;

            var inputErrorMessage = _editResponseViewModel.InputErrorMessage;

            inputErrorMessage.Should().Be("The same key is specified multiple times");
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.InputErrorMessage);
        }

        [Test]
        public void IfInvalidHeaderIsRemoved_InputErrorMessage_ShouldBeNull()
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Word(), FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(null, FakerWrapper.Faker.Lorem.Paragraph());
            using var monitor = _editResponseViewModel.Monitor();
            _editResponseViewModel.Headers.RemoveAt(1);

            var inputErrorMessage = _editResponseViewModel.InputErrorMessage;

            inputErrorMessage.Should().BeNull();
            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.InputErrorMessage);
        }

        [Test]
        public void IfHeaderIsRemoved_InputErrorMessage_ShouldNotRaiseTheEventPropertyChange()
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Word(), FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Word(), FakerWrapper.Faker.Lorem.Paragraph());
            var headerToBeRemoved = _editResponseViewModel.Headers[1];
            _editResponseViewModel.Headers.Remove(headerToBeRemoved);
            using var monitor = _editResponseViewModel.Monitor();
            headerToBeRemoved.Key = FakerWrapper.Faker.Lorem.Word();

            var unused = _editResponseViewModel.InputErrorMessage;

            monitor.Should().NotRaisePropertyChangeFor(viewModel => viewModel.InputErrorMessage);
        }

        #endregion

        #region AddHeader

        [Test]
        public void AddHeader_ShouldHadAHeader()
        {
            var expectedKey = FakerWrapper.Faker.Lorem.Paragraph();
            var expectedValue = FakerWrapper.Faker.Lorem.Paragraph();
            _editResponseViewModel.AddHeader(expectedKey, expectedValue);

            var header = _editResponseViewModel.Headers.Single();
            header.Key.Should().Be(expectedKey);
            header.Value.Should().Be(expectedValue);
            var (key, value) = _editResponseViewModel.HeadersDictionary.Single();
            key.Should().Be(expectedKey);
            value.Should().Be(expectedValue);
        }

        #endregion

        #region AddHeaderCommand

        [Test]
        public void AddHeaderCommand_ShouldHadAHeader()
        {
            ExecuteAddHeaderCommand();

            var header = _editResponseViewModel.Headers.Single();
            header.Key.Should().BeNull();
            header.Value.Should().BeNull();
        }

        #endregion

        #region DeleteHeader

        [Test]
        public void WhenDeletingAnAddedHeaderThatWasAddedWithTheRelativeCommand_ShouldRemoveOnlyThatHeader()
        {
            ExecuteAddHeaderCommand();
            var headerNotToBeDeleted = _editResponseViewModel.Headers.Single();
            ExecuteAddHeaderCommand();
            var headerToBeDeleted = _editResponseViewModel.Headers.Single(h => h != headerNotToBeDeleted);
            ExecuteAddHeaderCommand();
            ExecuteAddHeaderCommand();

            headerToBeDeleted.DeleteHeaderCommand.Execute(null);

            _editResponseViewModel.Headers.Should().HaveCount(3);
            _editResponseViewModel.Headers.Where(h => h == headerToBeDeleted).Should().BeEmpty();
        }

        [Test]
        public void WhenDeletingAnAddedHeaderThatWasAddedWithTheRelativeMethod_ShouldRemoveOnlyThatHeader()
        {
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            var headerNotToBeDeleted = _editResponseViewModel.Headers.Single();
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            var headerToBeDeleted = _editResponseViewModel.Headers.Single(h => h != headerNotToBeDeleted);
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());
            _editResponseViewModel.AddHeader(FakerWrapper.Faker.Lorem.Paragraph(), FakerWrapper.Faker.Lorem.Paragraph());

            headerToBeDeleted.DeleteHeaderCommand.Execute(null);

            _editResponseViewModel.Headers.Should().HaveCount(3);
            _editResponseViewModel.Headers.Where(h => h == headerToBeDeleted).Should().BeEmpty();
        }

        #endregion

        #region Utility Methods

        private void ExecuteAddHeaderCommand()
        {
            _editResponseViewModel.AddHeaderCommand.Execute(null);
        }

        #endregion
    }
}