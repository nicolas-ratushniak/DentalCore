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

namespace DentalCore.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ILogger<MainViewModel> _logger;
    private readonly UpdateOptions _updateConfiguration;

    private BaseViewModel? _currentPage;
    private BaseViewModel? _currentModal;
    private PageType _currentNavBarOption;
    private string _currentVersion = "?.?.?";

    public INavigationService NavigationService { get; }
    public IModalService ModalService { get; set; }

    public bool IsModalOpen => CurrentModal is not null;

    public BaseViewModel? CurrentPage
    {
        get => _currentPage;
        set
        {
            if (Equals(value, _currentPage)) return;
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public BaseViewModel? CurrentModal
    {
        get => _currentModal;
        set
        {
            if (Equals(value, _currentModal)) return;
            _currentModal = value;
            OnPropertyChanged();
        }
    }

    public PageType CurrentNavBarOption
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
        IModalService modalService,
        IOptions<UpdateOptions> updateOptions,
        ILogger<MainViewModel> logger)
    {
        NavigationService = navigationService;
        ModalService = modalService;
        
        _viewModelFactory = viewModelFactory;
        _logger = logger;
        _updateConfiguration = updateOptions.Value;

        NavigationService.CurrentPageTypeChanged += OnCurrentPageTypeChanged;
        NavigationService.NavigateTo(PageType.Patients);
        
        ModalService.CurrentModalTypeChanged += OnCurrentModalChanged;

        LoadedCommand = new AsyncRelayCommand(
            LoadDataAsync,
            _ => MessageBox.Show("Під час перевірки оновлень виникла помилка"));
    }

    public override void Dispose()
    {
        NavigationService.CurrentPageTypeChanged -= OnCurrentPageTypeChanged;
        base.Dispose();
    }

    public override async Task LoadDataAsync()
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

    private void OnCurrentPageTypeChanged(object? sender, PageTypeChangedEventArgs args)
    {
        var newViewType = args.NewPageType;

        CurrentPage = args.PageParameter is null
            ? _viewModelFactory.CreatePageViewModel(newViewType)
            : _viewModelFactory.CreatePageViewModel(newViewType, args.PageParameter);

        CurrentNavBarOption = newViewType switch
        {
            PageType.Patients or
                PageType.PatientCreate or
                PageType.PatientInfo or
                PageType.PatientUpdate => PageType.Patients,
            PageType.Visits or
                PageType.VisitCreate or
                PageType.VisitInfo => PageType.Visits,
            PageType.Procedures => PageType.Procedures,
            _ => throw new InvalidOperationException("Unknown view type passed")
        };
    }

    private void OnCurrentModalChanged(object? sender, ModalTypeChangedEventArgs args)
    {
        if (args.NewModalType is { } newModalType)
        {
            CurrentModal = args.ModalParameter is null
                ? _viewModelFactory.CreateModalViewModel(newModalType)
                : _viewModelFactory.CreateModalViewModel(newModalType, args.ModalParameter);
            
            OnPropertyChanged(nameof(IsModalOpen));
            return;
        }
        
        CurrentModal = null;

        if (args.NeedsPageReload)
        {
            CurrentPage?.LoadDataAsync();
        }
        
        OnPropertyChanged(nameof(IsModalOpen));
    }
}