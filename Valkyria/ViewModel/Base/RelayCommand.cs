using System;
using System.Windows.Input;

namespace Valkyrie.ViewModel
{
    class RelayCommand : ICommand
    {
        private Action command;

        public RelayCommand(Action action)
        {
            command = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            command.Invoke();
        }
    }
}
