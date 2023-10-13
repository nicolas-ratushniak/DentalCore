using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DentalCore.Wpf.Commands;

public abstract class AsyncCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    private readonly Action<Exception>? _onException;
    private bool _isExecuting;

    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    protected AsyncCommand(Action<Exception>? onException)
    {
        _onException = onException;
    }

    public bool CanExecute(object? parameter)
    {
        return !_isExecuting;
    }

    public async void Execute(object? parameter)
    {
        IsExecuting = true;

        try
        {
            await ExecuteAsync(parameter);
        }
        catch (Exception ex)
        {
            _onException?.Invoke(ex);
        }

        IsExecuting = false;
    }

    protected abstract Task ExecuteAsync(object? parameter);
}