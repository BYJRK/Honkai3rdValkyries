using System;
using System.Windows.Input;

namespace Valkyrie.ViewModel
{
    class RelayParamCommand : ICommand
    {
        private Action<object> command;

        public RelayParamCommand(Action<object> action)
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
            command.Invoke(parameter);
        }
    }
}
