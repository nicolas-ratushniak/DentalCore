using System;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Factories;

namespace DentalCore.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private BaseViewModel? _currentViewModel;
    private ViewType _currentNavBarOption;

    public INavigationService Navigator { get; }

    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (Equals(value, _currentViewModel)) return;
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public ViewType CurrentNavBarOption
    {
        get => _currentNavBarOption;
        set
        {
            if (value == _currentNavBarOption) return;
            _currentNavBarOption = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(IViewModelFactory viewModelFactory, INavigationService navigationService)
    {
        Navigator = navigationService;
        _viewModelFactory = viewModelFactory;
        
        Navigator.CurrentViewTypeChanged += OnCurrentViewTypeChanged;
        Navigator.NavigateTo(ViewType.Patients, null);
    }

    public override void Dispose()
    {
        Navigator.CurrentViewTypeChanged -= OnCurrentViewTypeChanged;
        base.Dispose();
    }

    private void OnCurrentViewTypeChanged(object? sender, ViewTypeChangedEventArgs args)
    {
        var newViewType = args.NewViewType;

        CurrentViewModel = _viewModelFactory.CreateViewModel(newViewType, args.ViewParameter);

        switch (newViewType)
        {
            case ViewType.Patients or ViewType.PatientCreate or ViewType.PatientInfo or ViewType.PatientUpdate:
                CurrentNavBarOption = ViewType.Patients;
                break;
            case ViewType.Visits or ViewType.VisitCreate or ViewType.VisitInfo or ViewType.VisitsExport:
                CurrentNavBarOption = ViewType.Visits;
                break;
            default:
                throw new InvalidOperationException("Unknown view type passed");
        }
    }
}