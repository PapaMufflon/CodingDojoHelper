using System;
using System.Windows.Input;

namespace CodingDojoHelper.Helper
{
    class DelegateCommand<T> : ICommand where T : class
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (parameter == null)
                return _canExecute(null);

            if (_canExecute == null)
                return true;

            var castedParameter = parameter as T;

            if (castedParameter == null)
                return true;

            return _canExecute(castedParameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                _execute(null);
                return;
            }

            var castedParameter = parameter as T;

            if (castedParameter == null)
                return;

            _execute(castedParameter);
        }

        #endregion
    }
}