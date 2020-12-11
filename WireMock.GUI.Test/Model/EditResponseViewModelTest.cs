using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #region Headers

        [Test]
        public void WhenModelIsInstantiated_Headers_ShouldReturnAnEmptyList()
        {
            _editResponseViewModel = new EditResponseViewModel();

            _editResponseViewModel.Headers.Should().BeEmpty();
        }

        [Test]
        public void Headers_ShouldBeEditableAndShouldAlsoAlignHeadersForGui()
        {
            var expectedHeaders = new ObservableCollection<HeaderViewModel>
            {
                new HeaderViewModel { Key = FakerWrapper.Faker.Lorem.Paragraph() },
                new HeaderViewModel  { Key = FakerWrapper.Faker.Lorem.Paragraph() }
            };
            var headersDictionary = ToDictionary(expectedHeaders);
            
            _editResponseViewModel.Headers = headersDictionary;

            _editResponseViewModel.Headers.Should().BeEquivalentTo(headersDictionary);
            _editResponseViewModel.HeadersForGui.Should().BeEquivalentTo(expectedHeaders);
        }

        #endregion

        #region HeadersForGui

        [Test]
        public void WhenModelIsInstantiated_HeadersForGui_ShouldReturnAnEmptyList()
        {
            _editResponseViewModel = new EditResponseViewModel();

            _editResponseViewModel.HeadersForGui.Should().BeEmpty();
        }

        #endregion

        #region AddHeader

        [Test]
        public void AddHeader_ShouldHadAHeader()
        {
            ExecuteAddHeaderCommand();

            var header = _editResponseViewModel.HeadersForGui[0];
            header.Key.Should().BeNull();
            header.Value.Should().BeNull();
        }

        #endregion

        #region DeleteHeader

        [Test]
        public void WhenDeleteAnAddedHeader_ShouldRemoveOnlyThatHeader()
        {
            ExecuteAddHeaderCommand();
            var headerNotToBeDeleted = _editResponseViewModel.HeadersForGui.Single();
            ExecuteAddHeaderCommand();
            var headerToBeDeleted = _editResponseViewModel.HeadersForGui.Single(h => h != headerNotToBeDeleted);
            ExecuteAddHeaderCommand();
            ExecuteAddHeaderCommand();

            headerToBeDeleted.DeleteHeaderCommand.Execute(null);

            _editResponseViewModel.HeadersForGui.Should().HaveCount(3);
            _editResponseViewModel.HeadersForGui.Where(h => h == headerToBeDeleted).Should().BeEmpty();
        }

        #endregion

        #region Utility Methods

        private void ExecuteAddHeaderCommand()
        {
            _editResponseViewModel.AddHeaderCommand.Execute(null);
        }

        private static IDictionary<string, string> ToDictionary(IEnumerable<HeaderViewModel> headers)
        {
            return headers.ToDictionary(header => header.Key, header => header.Value);
        }

        #endregion
    }
}