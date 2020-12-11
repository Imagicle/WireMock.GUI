using System.Windows.Input;
using WireMock.GUI.WPF;

namespace WireMock.GUI.Model
{
    public class HeaderViewModel : ViewModel
    {
        #region Private fields
        
        private string _key;
        private string _value;

        #endregion

        #region Initialization

        public HeaderViewModel()
        {
            DeleteHeaderCommand = new RelayCommand(o => ExecuteDeleteHeader(), o => true, this);
        }

        #endregion

        #region Events

        public delegate void DeleteHeader(HeaderViewModel mapping);
        public event DeleteHeader OnDeleteHeader;

        #endregion

        #region Commands

        public ICommand DeleteHeaderCommand { get; }

        #endregion

        #region Properties

        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        #endregion

        #region Utility Methods

        private void ExecuteDeleteHeader()
        {
            OnDeleteHeader?.Invoke(this);
        }

        #endregion
    }
}