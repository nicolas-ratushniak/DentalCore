using System;
using System.Windows.Input;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;

namespace DentalCore.Wpf.Services;

public class ModalService : IModalService
{
    public event EventHandler<ModalTypeChangedEventArgs>? CurrentModalTypeChanged;
    public ModalType? CurrentModalType { get; private set; }
    public ICommand UpdateCurrentModalTypeCommand { get; }

    public ModalService()
    {
        UpdateCurrentModalTypeCommand = new RelayCommand<ModalType>(OpenModal);
    }

    public void OpenModal(ModalType newModalType)
    {
        var oldModalType = CurrentModalType;

        if (oldModalType == newModalType)
        {
            return;
        }

        CurrentModalType = newModalType;
        CurrentModalTypeChanged?.Invoke(
            this, 
            new ModalTypeChangedEventArgs(oldModalType, newModalType, false));
    }

    public void OpenModal(ModalType newModalType, object modalParameter)
    {
        var oldModalType = CurrentModalType;

        if (oldModalType == newModalType)
        {
            return;
        }

        CurrentModalType = newModalType;
        CurrentModalTypeChanged?.Invoke(
            this, 
            new ModalTypeChangedEventArgs(oldModalType, newModalType, false, modalParameter));
    }

    public void CloseModal()
    {
        if (CurrentModalType is null)
        {
            return;
        }
        
        CurrentModalType = null;
        CurrentModalTypeChanged?.Invoke(
            this, 
            new ModalTypeChangedEventArgs(CurrentModalType, null, false));
    }

    public void CloseModalWithPageReload()
    {
        if (CurrentModalType is null)
        {
            return;
        }
        
        CurrentModalType = null;
        CurrentModalTypeChanged?.Invoke(
            this, 
            new ModalTypeChangedEventArgs(CurrentModalType, null, true));
    }
}