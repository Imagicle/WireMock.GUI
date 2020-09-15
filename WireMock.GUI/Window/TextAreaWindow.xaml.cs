using System.Windows;

namespace WireMock.GUI.Window
{
    public partial class TextAreaWindow : ITextAreaWindow
    {
        #region Initialization

        public TextAreaWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public string InputValue
        {
            get => ResponseTextBox.Text;
            set => ResponseTextBox.Text = value;
        }

        #endregion

        #region Methods

        public new bool ShowDialog()
        {
            return base.ShowDialog() == true;
        }

        #endregion

        #region Utility Methods

        private void OkButtonCommand(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion
    }
}
