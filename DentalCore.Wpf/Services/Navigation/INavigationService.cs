using System;
using System.Windows.Input;

namespace DentalCore.Wpf.Services.Navigation;

public interface INavigationService
{
    public event EventHandler<ViewTypeChangedEventArgs> CurrentViewTypeChanged;
    public ViewType? CurrentViewType { get; }
    public ICommand UpdateCurrentViewTypeCommand { get; }
    public void NavigateTo(ViewType newViewType, object? viewParameter);
}