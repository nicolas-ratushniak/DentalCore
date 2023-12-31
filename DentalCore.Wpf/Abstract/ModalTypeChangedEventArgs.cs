namespace DentalCore.Wpf.Abstract;

public class ModalTypeChangedEventArgs
{
    public ModalType? OldModalType { get; set; }
    public ModalType? NewModalType { get; set; }
    public bool NeedsPageReload { get; set; }
    public object? ModalParameter { get; set; }

    public ModalTypeChangedEventArgs(ModalType? oldModalType, ModalType? newModalType, bool needsPageReload)
    {
        OldModalType = oldModalType;
        NewModalType = newModalType;
        NeedsPageReload = needsPageReload;
        ModalParameter = null;
    }
    
    public ModalTypeChangedEventArgs(ModalType? oldModalType, ModalType? newModalType, bool needsPageReload, object modalParameter)
    {
        OldModalType = oldModalType;
        NewModalType = newModalType;
        NeedsPageReload = needsPageReload;
        ModalParameter = modalParameter;
    }
}