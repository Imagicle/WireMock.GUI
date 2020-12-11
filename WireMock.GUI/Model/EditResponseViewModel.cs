using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WireMock.GUI.WPF;

namespace WireMock.GUI.Model
{
    public class EditResponseViewModel : ViewModel
    {
        #region Private fields

        private string _body;

        #endregion

        #region Initialization

        public EditResponseViewModel()
        {
            AddHeaderCommand = new RelayCommand(o => ExecuteAddHeader(), o => true, this);
            HeadersForGui = new ObservableCollection<HeaderViewModel>();
        }

        #endregion

        #region Commands

        public ICommand AddHeaderCommand { get; }

        #endregion

        #region Properties

        public string Body
        {
            get => _body;
            set
            {
                _body = value;
                OnPropertyChanged(nameof(Body));
            }
        }

        public IDictionary<string, string> Headers
        {
            get => ToDictionary(HeadersForGui);
            set => ToObservableCollection(value);
        }

        public ObservableCollection<HeaderViewModel> HeadersForGui { get; private set; }

        #endregion

        #region Utility Methods

        private void ExecuteAddHeader()
        {
            AddHeader(null, null);
        }

        private void AddHeader(string key, string value)
        {
            var headerViewModel = new HeaderViewModel
            {
                Key = key,
                Value = value
            };
            headerViewModel.OnDeleteHeader += OnDeleteHeader;
            HeadersForGui.Add(headerViewModel);
        }

        private void OnDeleteHeader(HeaderViewModel header)
        {
            HeadersForGui.Remove(header);
        }

        private static IDictionary<string, string> ToDictionary(IEnumerable<HeaderViewModel> headers)
        {
            return headers.ToDictionary(header => header.Key, header => header.Value);
        }

        private void ToObservableCollection(IDictionary<string, string> headers)
        {
            HeadersForGui = new ObservableCollection<HeaderViewModel>();
            foreach (var (key, value) in headers)
            {
                AddHeader(key, value);
            }
        }

        #endregion
    }
}