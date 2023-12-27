using System;
using System.Windows.Input;

namespace DentalCore.Wpf.Abstract;

public interface INavigationService
{
    public event EventHandler<PageTypeChangedEventArgs> CurrentPageTypeChanged;
    public PageType? CurrentPageType { get; }
    public ICommand UpdateCurrentPageTypeCommand { get; }
    public void NavigateTo(PageType newPageType);
    public void NavigateTo(PageType newPageType, object pageParameter);
}