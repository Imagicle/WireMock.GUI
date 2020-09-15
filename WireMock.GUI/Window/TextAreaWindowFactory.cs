namespace WireMock.GUI.Window
{
    internal class TextAreaWindowFactory : ITextAreaWindowFactory
    {
        public ITextAreaWindow Create()
        {
            return new TextAreaWindow();
        }
    }
}