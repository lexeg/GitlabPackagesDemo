using System;
using System.Windows.Input;

namespace GitlabPackagesDemo.Commands;

public class BaseAutoEventCommand : ICommand
{
    private readonly Action<object> _action;
    private readonly Func<object, bool> _predicate;

    public BaseAutoEventCommand(Action<object> action, Func<object, bool> predicate)
    {
        _action = action;
        _predicate = predicate;
    }

    public bool CanExecute(object parameter) => _predicate(parameter);

    public void Execute(object parameter)
    {
        _action(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}