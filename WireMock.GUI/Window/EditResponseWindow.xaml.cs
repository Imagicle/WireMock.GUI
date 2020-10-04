using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public IDictionary<string, string> Headers
        {

            get => ToDictionary(_viewModel.Headers);
            set => _viewModel.Headers = ToViewModel(value);
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

        private static IDictionary<string, string> ToDictionary(IEnumerable<HeaderViewModel> headers)
        {
            return headers.ToDictionary(header => header.Key, header => header.Value);
        }

        private static ObservableCollection<HeaderViewModel> ToViewModel(IDictionary<string, string> headers)
        {
            var result = new ObservableCollection<HeaderViewModel>();
            foreach (var header in headers)
            {
                result.Add(new HeaderViewModel
                {
                    Key = header.Key,
                    Value = header.Value
                });
            }

            return result;
        }

        #endregion
    }
}
