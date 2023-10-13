using System;
using System.Threading.Tasks;

namespace DentalCore.Wpf.Commands;

public class AsyncRelayCommand : AsyncCommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, Action<Exception>? onException = null) :
        base(onException)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter) && _canExecute();
    }

    protected override async Task ExecuteAsync(object? parameter)
    {
        await _execute();
    }
}