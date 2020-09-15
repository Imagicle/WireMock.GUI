using System.ComponentModel;
using WireMock.GUI.Mapping;
using WireMock.GUI.Mock;
using WireMock.GUI.Model;

namespace WireMock.GUI
{
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var mockServer = new WireMockWrapper();
            var mappings = new MappingsProvider();
            _viewModel = new MainWindowViewModel(mockServer, mappings);

            DataContext = _viewModel;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _viewModel.Stop();
            base.OnClosing(e);
        }
    }
}
