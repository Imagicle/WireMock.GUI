using System.Collections.Generic;
using System.Windows;
using WireMock.GUI.Model;

namespace WireMock.GUI.Window
{
    public partial class EditResponseWindow : IEditResponseWindow
    {
        #region Private fields

        private readonly EditResponseViewModel _viewModel;

        #endregion

        #region Initialization

        public EditResponseWindow()
        {
            InitializeComponent();

            _viewModel = new EditResponseViewModel();

            DataContext = _viewModel;
        }

        #endregion

        #region Properties

        public string Body
        {
            get => _viewModel.Body;
            set => _viewModel.Body = value;
        }

        public IDictionary<string, string> Headers => _viewModel.HeadersDictionary;

        #endregion

        #region Methods

        public void AddHeader(string key, string value)
        {
            _viewModel.AddHeader(key, value);
        }

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
