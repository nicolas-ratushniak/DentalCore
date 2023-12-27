using System;
using System.Windows.Input;

namespace DentalCore.Wpf.Abstract;

public interface INavigationService
{
    public event EventHandler<ViewTypeChangedEventArgs> CurrentViewTypeChanged;
    public ViewType? CurrentViewType { get; }
    public ICommand UpdateCurrentViewTypeCommand { get; }
    public void NavigateTo(ViewType newViewType, object? viewParameter);
}