using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WireMock.GUI.WPF
{
    // TODO: use IMALIB-52 when done
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> action, Predicate<object> canExecute, INotifyPropertyChanged notifyingObject = null)
        {
            _action = action;
            _canExecute = canExecute;
            CanExecuteChangeOnPropertyChange(notifyingObject);
        }

        private void CanExecuteChangeOnPropertyChange(INotifyPropertyChanged notifyingObject)
        {
            if (notifyingObject != null)
            {
                notifyingObject.PropertyChanged += (sender, args) => OnCanExecuteChanged();
            }
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;
        private void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                Application.Current?.Dispatcher.Invoke(
                    DispatcherPriority.Render,
                    new Action(() => handler(this, EventArgs.Empty)));
            }
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        #endregion
    }
}