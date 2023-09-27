using DentalCore.Wpf.Services.Navigation;

namespace DentalCore.Wpf.ViewModels.Factories;

public interface IViewModelFactory
{
    public BaseViewModel CreateViewModel(ViewType viewType, object? viewParameter = null);
}