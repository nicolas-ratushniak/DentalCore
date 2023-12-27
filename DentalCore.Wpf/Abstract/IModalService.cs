using System;
using System.Windows.Input;

namespace DentalCore.Wpf.Abstract;

public interface IModalService
{
    public event EventHandler<ModalTypeChangedEventArgs> CurrentModalTypeChanged;
    public ModalType? CurrentModalType { get; }
    public ICommand UpdateCurrentModalTypeCommand { get; }
    public void OpenModal(ModalType newModalType);
    public void OpenModal(ModalType newModalType, object modalParameter);
    public void CloseModal();
    public void CloseModalWithPageReload();
}