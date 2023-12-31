using System;

namespace DentalCore.Wpf.Abstract;

public class PageTypeChangedEventArgs : EventArgs
{
    public PageType? OldViewType { get; set; }
    public PageType NewPageType { get; set; }
    public object? PageParameter { get; set; }

    public PageTypeChangedEventArgs(PageType? oldViewType, PageType newPageType)
    {
        OldViewType = oldViewType;
        NewPageType = newPageType;
        PageParameter = null;
    }
    
    public PageTypeChangedEventArgs(PageType? oldViewType, PageType newPageType, object pageParameter)
    {
        OldViewType = oldViewType;
        NewPageType = newPageType;
        PageParameter = pageParameter;
    }
}