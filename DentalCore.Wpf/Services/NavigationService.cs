using System;
using System.Windows.Input;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Commands.Generic;

namespace DentalCore.Wpf.Services;

public class NavigationService : INavigationService
{
    public event EventHandler<PageTypeChangedEventArgs>? CurrentPageTypeChanged;

    public PageType? CurrentPageType { get; private set; }
    public ICommand UpdateCurrentPageTypeCommand { get; }

    public NavigationService()
    {
        UpdateCurrentPageTypeCommand = new RelayCommand<PageType>(NavigateTo);
    }

    public void NavigateTo(PageType newPageType)
    {
        var oldViewType = CurrentPageType;

        if (oldViewType == newPageType)
        {
            return;
        }
        
        CurrentPageType = newPageType;
        CurrentPageTypeChanged?.Invoke(this, new PageTypeChangedEventArgs(oldViewType, newPageType));
    }

    public void NavigateTo(PageType newPageType, object pageParameter)
    {
        var oldViewType = CurrentPageType;

        if (oldViewType == newPageType)
        {
            return;
        }
        
        CurrentPageType = newPageType;
        CurrentPageTypeChanged?.Invoke(this, new PageTypeChangedEventArgs(oldViewType, newPageType, pageParameter));
    }
}