using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Configuration;
using DentalCore.Wpf.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DentalCore.Wpf.ViewModels.Pages;

public class MainViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ILogger<MainViewModel> _logger;
    private readonly UpdateOptions _updateConfiguration;

    private BaseViewModel? _currentViewModel;
    private ViewType _currentNavBarOption;
    private string _currentVersion = "?.?.?";

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

    public string CurrentVersion
    {
        get => _currentVersion;
        set
        {
            if (value == _currentVersion) return;
            _currentVersion = value;
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

        Navigator.CurrentViewTypeChanged += OnCurrentViewTypeChanged;
        Navigator.NavigateTo(ViewType.Patients, null);

        LoadedCommand = new AsyncRelayCommand(
            LoadData,
            _ => MessageBox.Show("Під час перевірки оновлень виникла помилка"));
    }

    public override void Dispose()
    {
        Navigator.CurrentViewTypeChanged -= OnCurrentViewTypeChanged;
        base.Dispose();
    }

    private async Task LoadData()
    {
        try
        {
            using var updateManager = await GithubUpdateManager.CreateAsync(
                _updateConfiguration.GitHubRepo, "appsettings.json");
            
            CurrentVersion = updateManager.GetCurrentVersionInstalled(CurrentVersion);
                
            if (await updateManager.HasNewerReleaseAsync())
            {
                _logger.LogInformation("Updates are found! Starting the download...");
                await updateManager.UpdateAsync();
                _logger.LogInformation("Updates were installed");
            }
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

    private void OnCurrentViewTypeChanged(object? sender, ViewTypeChangedEventArgs args)
    {
        var newViewType = args.NewViewType;

        CurrentViewModel = _viewModelFactory.CreateViewModel(newViewType, args.ViewParameter);

        CurrentNavBarOption = newViewType switch
        {
            ViewType.Patients or ViewType.PatientCreate or ViewType.PatientInfo or ViewType.PatientUpdate => ViewType.Patients,
            ViewType.Visits or ViewType.VisitCreate or ViewType.VisitInfo or ViewType.VisitsExport => ViewType.Visits,
            ViewType.Procedures => ViewType.Procedures,
            _ => throw new InvalidOperationException("Unknown view type passed")
        };
    }
}