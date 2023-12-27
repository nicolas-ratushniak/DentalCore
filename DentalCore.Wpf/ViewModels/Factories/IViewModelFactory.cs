using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.ViewModels.Factories;

public interface IViewModelFactory
{
    public BaseViewModel CreateViewModel(ViewType viewType, object? viewParameter = null);
}