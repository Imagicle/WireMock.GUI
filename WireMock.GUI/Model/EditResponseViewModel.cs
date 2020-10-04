using System.Collections.ObjectModel;
using System.Windows.Input;
using WireMock.GUI.WPF;

namespace WireMock.GUI.Model
{
    public class EditResponseViewModel : ViewModel
    {
        private string _body;

        public EditResponseViewModel()
        {
            AddHeaderCommand = new RelayCommand(o => ExecuteAddHeader(), o => true, this);
            Headers = new ObservableCollection<HeaderViewModel>();
        }

        public ICommand AddHeaderCommand { get; }

        public string Body
        {
            get => _body;
            set
            {
                _body = value;
                OnPropertyChanged(nameof(Body));
            }
        }

        public ObservableCollection<HeaderViewModel> Headers { get; set; }

        #region Utility Methods

        private void ExecuteAddHeader()
        {
            Headers.Add(new HeaderViewModel());
        }

        #endregion
    }
}