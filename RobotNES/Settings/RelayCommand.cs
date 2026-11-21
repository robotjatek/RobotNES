using System;
using System.Windows.Input;

namespace RobotNES.Settings;

/// <summary>
/// Minimal command that forwards its typed parameter to an action. WPF passes the bound
/// CommandParameter as object, so the value is converted to <typeparamref name="T"/> on the way in.
/// </summary>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;

    public RelayCommand(Action<T?> execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        _execute(parameter is T typed ? typed : default);
    }
}
