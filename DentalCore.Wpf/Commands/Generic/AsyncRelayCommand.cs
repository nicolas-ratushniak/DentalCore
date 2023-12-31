using System;
using System.Threading.Tasks;
using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.Commands.Generic;

public class AsyncRelayCommand<T> : BaseAsyncCommand
{
    private readonly Func<T, Task> _execute;

    public AsyncRelayCommand(Func<T, Task> execute, Action<Exception>? onException = null) : base(onException)
    {
        _execute = execute;
    }

    protected override async Task ExecuteAsync(object? parameter)
    {
        await _execute((T)parameter!);
    }
}