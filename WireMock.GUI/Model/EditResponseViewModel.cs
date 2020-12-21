using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using WireMock.GUI.WPF;

namespace WireMock.GUI.Model
{
    public class EditResponseViewModel : ViewModel
    {
        #region Private fields

        private string _body;
        private bool _isInputInvalid;
        private string _inputErrorMessage;

        #endregion

        #region Initialization

        public EditResponseViewModel()
        {
            AddHeaderCommand = new RelayCommand(o => ExecuteAddHeader(), o => true, this);
            Headers = new ObservableCollection<HeaderViewModel>();
            Headers.CollectionChanged += ContentCollectionChanged;
            ValidateHeaders();
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

        public IDictionary<string, string> HeadersDictionary => ToDictionary(Headers);

        public ObservableCollection<HeaderViewModel> Headers { get; }

        public bool IsInputValid
        {
            get => _isInputInvalid;
            private set
            {
                _isInputInvalid = value;
                OnPropertyChanged(nameof(IsInputValid));
            }
        }

        public string InputErrorMessage
        {
            
            get => _inputErrorMessage;
            private set
            {
                _inputErrorMessage = value;
                OnPropertyChanged(nameof(InputErrorMessage));
            }
        }

        #endregion

        #region Methods

        public void AddHeader(string key, string value)
        {
            var headerViewModel = new HeaderViewModel
            {
                Key = key,
                Value = value
            };
            headerViewModel.OnDeleteHeader += OnDeleteHeader;
            Headers.Add(headerViewModel);
        }

        #endregion

        #region Utility Methods

        private void ExecuteAddHeader()
        {
            AddHeader(null, null);
        }

        private void OnDeleteHeader(HeaderViewModel header)
        {
            Headers.Remove(header);
        }

        private void ValidateHeaders()
        {
            IsInputValid = true;
            InputErrorMessage = null;

            if (Headers.Any(model => string.IsNullOrWhiteSpace(model.Key)))
            {
                IsInputValid = false;
                InputErrorMessage = "Null or empty header keys are not allowed";
            } 
            else if (Headers.GroupBy(model => model.Key).Any(h => h.Count() > 1))
            {
                IsInputValid = false;
                InputErrorMessage = "The same key is specified multiple times";
            }
        }

        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (HeaderViewModel item in e.NewItems)
                {
                    item.PropertyChanged += EntityViewModelPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (HeaderViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= EntityViewModelPropertyChanged;
                }
            }

            ValidateHeaders();
        }

        private void EntityViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ValidateHeaders();
        }

        private static IDictionary<string, string> ToDictionary(IEnumerable<HeaderViewModel> headers)
        {
            return headers.ToDictionary(header => header.Key, header => header.Value);
        }

        #endregion
    }
}