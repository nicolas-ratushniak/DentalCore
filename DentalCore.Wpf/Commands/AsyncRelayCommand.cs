using System;
using System.Threading.Tasks;

namespace DentalCore.Wpf.Commands;

public class AsyncRelayCommand : AsyncCommand
{
    private readonly Func<Task> _execute;

    public AsyncRelayCommand(Func<Task> execute, Action<Exception>? onException = null) :
        base(onException)
    {
        _execute = execute;
    }

    protected override async Task ExecuteAsync(object? parameter)
    {
        await _execute();
    }
}