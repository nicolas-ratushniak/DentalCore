namespace DentalCore.Wpf.Abstract;

public interface IViewModelFactory
{
    public BaseViewModel CreatePageViewModel(PageType pageType);
    public BaseViewModel CreatePageViewModel(PageType pageType, object viewParameter);
    public BaseViewModel CreateModalViewModel(ModalType modalType);
    public BaseViewModel CreateModalViewModel(ModalType modalType, object modalParameter);
}