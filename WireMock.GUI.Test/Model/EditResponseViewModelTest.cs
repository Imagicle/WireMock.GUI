using System.Collections.ObjectModel;
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

        #region Headers

        [Test]
        public void WhenModelIsInstantiated_Headers_ShouldReturnAnEmptyList()
        {
            _editResponseViewModel = new EditResponseViewModel();

            _editResponseViewModel.Headers.Should().BeEmpty();
        }

        [Test]
        public void Headers_ShouldBeEditable()
        {
            var expectedHeaders = new ObservableCollection<HeaderViewModel>
            {
                new HeaderViewModel(),
                new HeaderViewModel()
            };

            _editResponseViewModel.Headers = expectedHeaders;

            _editResponseViewModel.Headers.Should().BeEquivalentTo(expectedHeaders);
        }

        #endregion

        #region AddHeader

        [Test]
        public void AddHeader_ShouldHadAHeader()
        {
            ExecuteAddHeaderCommand();

            var header = _editResponseViewModel.Headers[0];
            header.Key.Should().BeNull();
            header.Value.Should().BeNull();
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