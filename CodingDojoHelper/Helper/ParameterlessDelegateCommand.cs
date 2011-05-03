using System;

namespace CodingDojoHelper.Helper
{
    class ParameterlessDelegateCommand : DelegateCommand<object>
    {
        public ParameterlessDelegateCommand(Action execute, Func<bool> canExecute = null) :
            base(o => execute.Invoke(), o => canExecute == null ? true : canExecute.Invoke()) {}
    }
}