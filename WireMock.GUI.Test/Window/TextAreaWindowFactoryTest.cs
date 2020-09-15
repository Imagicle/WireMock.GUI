using FluentAssertions;
using NUnit.Framework;
using WireMock.GUI.Test.TestUtils;
using WireMock.GUI.Window;

namespace WireMock.GUI.Test.Window
{
    [TestFixture]
    public class TextAreaWindowFactoryTest
    {
        #region Fixture
        
        private ITextAreaWindowFactory _textAreaWindowFactory;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _textAreaWindowFactory = new TextAreaWindowFactory();
        }

        #endregion

        [Test]
        public void Create_ShouldReturnATextAreaWindow()
        {
            CommonTestUtils.RunInStaThread(() =>
            {
                var textAreaWindow = _textAreaWindowFactory.Create();

                textAreaWindow.Should().BeOfType<TextAreaWindow>();
            });
        }

        [Test]
        public void Create_ShouldReturnAlwaysANewInstance()
        {
            CommonTestUtils.RunInStaThread(() =>
            {
                var textAreaWindow1 = _textAreaWindowFactory.Create();
                var textAreaWindow2 = _textAreaWindowFactory.Create();

                textAreaWindow1.Should().NotBe(textAreaWindow2);
            });
        }
    }
}