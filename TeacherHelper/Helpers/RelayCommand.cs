using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TeacherHelper.Helpers
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            this._execute = execute;
            this.canExecuteCommand = true;
        }

        private event EventHandler canExecuteChanged;
        public event EventHandler CanExecuteChanged
        {
            add { canExecuteChanged += value; }
            remove { canExecuteChanged -= value; }
        }

        private bool canExecuteCommand;
        public bool CanExecuteCommand
        {
            get
            {
                return canExecuteCommand;
            }
            set
            {
                canExecuteCommand = value;
                canExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteCommand;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
