using DentalCore.Data.Models;
using DentalCore.Wpf.Services.Authentication;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Factories;

namespace DentalCore.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IAuthenticationService _authenticationService;
    private User? _currentUser;
    private BaseViewModel _currentViewModel;
    private ViewType _currentNavBarOption;

    public INavigationService Navigator { get; set; }
    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (Equals(value, _currentUser)) return;
            _currentUser = value;
            OnPropertyChanged();
        }
    }

    public BaseViewModel CurrentViewModel
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

    public MainViewModel(IViewModelFactory viewModelFactory, INavigationService navigationService, IAuthenticationService authenticationService)
    {
        _viewModelFactory = viewModelFactory;
        Navigator = navigationService;
        _authenticationService = authenticationService;
        
        Navigator.CurrentViewTypeChanged += OnCurrentViewTypeChanged;

        CurrentViewModel = _viewModelFactory.CreateViewModel(ViewType.Login);
    }

    private void OnCurrentViewTypeChanged(object? sender, ViewTypeChangedEventArgs args)
    {
        var newViewType = args.NewViewType;

        CurrentViewModel = _viewModelFactory.CreateViewModel(newViewType, args.ViewParameter);

        CurrentNavBarOption = newViewType switch
        {
            ViewType.Patients or ViewType.PatientCreate or ViewType.PatientInfo or ViewType.PatientUpdate 
                => ViewType.Patients,
            ViewType.Visits or ViewType.VisitCreate or ViewType.VisitInfo => ViewType.Visits,
            _ => ViewType.Home
        };
    }
}