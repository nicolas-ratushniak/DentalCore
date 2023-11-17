using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Configuration;
using DentalCore.Wpf.Services;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DentalCore.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ILogger<MainViewModel> _logger;
    private readonly UpdateOptions _updateConfiguration;
    private readonly GithubUpdateManager _updateManager;
    
    private BaseViewModel? _currentViewModel;
    private ViewType _currentNavBarOption;
    private bool _hasUpdatesToApply;

    public INavigationService Navigator { get; }
    public ICommand UpdateApplicationCommand { get; }

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

    public bool HasUpdatesToApply
    {
        get => _hasUpdatesToApply;
        set
        {
            if (value == _hasUpdatesToApply) return;
            _hasUpdatesToApply = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(
        IViewModelFactory viewModelFactory, 
        INavigationService navigationService,
        IOptions<UpdateOptions> updateOptions,
        ILogger<MainViewModel> logger)
    {
        Navigator = navigationService;
        _viewModelFactory = viewModelFactory;
        _logger = logger;
        _updateConfiguration = updateOptions.Value;
        _updateManager = new GithubUpdateManager();

        Navigator.CurrentViewTypeChanged += OnCurrentViewTypeChanged;
        Navigator.NavigateTo(ViewType.Patients, null);

        LoadedCommand = new AsyncRelayCommand(
            CheckForUpdates,
            _ => MessageBox.Show("Під час завантаження контенту виникла помилка"));
        
        UpdateApplicationCommand = new AsyncRelayCommand(
            UpdateApp_Execute,  
            _ => MessageBox.Show("Під час спроби оновлення виникла помилка"));
    }

    public override void Dispose()
    {
        Navigator.CurrentViewTypeChanged -= OnCurrentViewTypeChanged;
        base.Dispose();
    }

    private async Task CheckForUpdates()
    {
        try
        {
            await _updateManager.InitAsync(_updateConfiguration.GitHubRepo, "appsettings.json");
            HasUpdatesToApply = await _updateManager.HasNewerReleaseAsync();
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("Failed to check for updates. No releases were found");
        }
        catch (HttpRequestException)
        {
            _logger.LogWarning("Failed to check for updates. Http request failed");
        }
    }

    private async Task UpdateApp_Execute()
    {
        await _updateManager.UpdateAsync(restart: true);
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