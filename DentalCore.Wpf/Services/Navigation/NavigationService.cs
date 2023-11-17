using System;
using System.Windows.Input;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;

namespace DentalCore.Wpf.Services.Navigation;

public class NavigationService : INavigationService
{
    public event EventHandler<ViewTypeChangedEventArgs>? CurrentViewTypeChanged;

    public ViewType? CurrentViewType { get; private set; }
    public ICommand UpdateCurrentViewTypeCommand { get; }

    public NavigationService()
    {
        UpdateCurrentViewTypeCommand = new RelayCommand<ViewType>(
            v => NavigateTo(v, null));
    }

    public void NavigateTo(ViewType newViewType, object? viewParameter)
    {
        var oldViewType = CurrentViewType;

        if (oldViewType == newViewType)
        {
            return;
        }
        
        CurrentViewType = newViewType;
        CurrentViewTypeChanged?.Invoke(this, new ViewTypeChangedEventArgs(oldViewType, newViewType, viewParameter));
    }
}