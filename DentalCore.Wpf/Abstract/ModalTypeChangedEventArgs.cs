namespace DentalCore.Wpf.Abstract;

public class ModalTypeChangedEventArgs
{
    public ModalType? OldModalType { get; set; }
    public ModalType? NewModalType { get; set; }
    public object? ModalParameter { get; set; }

    public ModalTypeChangedEventArgs(ModalType? oldModalType, ModalType? newModalType)
    {
        OldModalType = oldModalType;
        NewModalType = newModalType;
        ModalParameter = null;
    }
    
    public ModalTypeChangedEventArgs(ModalType? oldModalType, ModalType? newModalType, object modalParameter)
    {
        OldModalType = oldModalType;
        NewModalType = newModalType;
        ModalParameter = modalParameter;
    }
}