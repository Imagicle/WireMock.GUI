namespace WireMock.GUI.Window
{
    internal class TextAreaWindowFactory : IEditResponseWindowFactory
    {
        public IEditResponseWindow Create()
        {
            return new EditResponseWindow();
        }
    }
}