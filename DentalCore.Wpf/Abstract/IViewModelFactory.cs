namespace DentalCore.Wpf.Abstract;

public interface IViewModelFactory
{
    public BaseViewModel CreateViewModel(ViewType viewType, object? viewParameter = null);
}