using FluentAssertions;
using NUnit.Framework;
using WireMock.GUI.Model;
using WireMock.GUI.Test.TestUtils;

namespace WireMock.GUI.Test.Model
{
    [TestFixture]
    public class HeaderViewModelTest
    {
        #region Fixture

        private HeaderViewModel _headerViewModel;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _headerViewModel = new HeaderViewModel();
        }

        #endregion

        #region Key

        [Test]
        public void WhenKeyIsModified_Key_ShouldRaiseAnEvent()
        {
            var expectedKey = FakerWrapper.Faker.Lorem.Paragraph();
            using var monitor = _headerViewModel.Monitor();

            _headerViewModel.Key = expectedKey;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.Key);
        }

        [Test]
        public void Key_ShouldBeEditable()
        {
            var expectedKey = FakerWrapper.Faker.Lorem.Paragraph();

            _headerViewModel.Key = expectedKey;

            _headerViewModel.Key.Should().Be(expectedKey);
        }

        #endregion

        #region Value

        [Test]
        public void WhenValueIsModified_Value_ShouldRaiseAnEvent()
        {
            var expectedValue = FakerWrapper.Faker.Lorem.Paragraph();
            using var monitor = _headerViewModel.Monitor();

            _headerViewModel.Value = expectedValue;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.Value);
        }

        [Test]
        public void Value_ShouldBeEditable()
        {
            var expectedValue = FakerWrapper.Faker.Lorem.Paragraph();

            _headerViewModel.Value = expectedValue;

            _headerViewModel.Value.Should().Be(expectedValue);
        }

        #endregion
    }
}