using System;
using System.Threading.Tasks;

namespace DentalCore.Wpf.Commands;

public class AsyncCommand : BaseAsyncCommand
{
    private readonly Func<Task> _execute;

    public AsyncCommand(Func<Task> execute, Action<Exception>? onException = null) : base(onException)
    {
        _execute = execute;
    }

    protected override async Task ExecuteAsync(object? parameter)
    {
        await _execute();
    }
}