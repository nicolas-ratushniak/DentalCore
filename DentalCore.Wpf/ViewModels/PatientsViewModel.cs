using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.DataExportServices;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Inners;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DentalCore.Wpf.ViewModels;

public class PatientsViewModel : BaseViewModel
{
    private readonly IExportService _exportService;
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientsViewModel> _logger;
    private string _patientSearchFilter = string.Empty;
    private readonly ObservableCollection<PatientListItemViewModel> _patients;
    private readonly ExportOptions _exportOptions;

    public ICommand AddPatientCommand { get; }
    public ICommand EditPatientCommand { get; }
    public ICommand ShowPatientCommand { get; }
    public ICommand ExportVisitsCommand { get; }

    public ICollectionView PatientCollectionView { get; }

    public string PatientSearchFilter
    {
        get => _patientSearchFilter;
        set
        {
            if (value == _patientSearchFilter) return;
            _patientSearchFilter = value;

            OnPropertyChanged();
            PatientCollectionView.Refresh();
        }
    }

    public PatientsViewModel(
        IOptions<ExportOptions> exportOptions,
        IExportService exportService,
        INavigationService navigationService, 
        IPatientService patientService,
        ILogger<PatientsViewModel> logger)
    {
        _exportOptions = exportOptions.Value;
        _exportService = exportService;
        _patientService = patientService;
        _logger = logger;
        _patients = new ObservableCollection<PatientListItemViewModel>();

        PatientCollectionView = CollectionViewSource.GetDefaultView(_patients);
        
        PatientCollectionView.SortDescriptions.Add(
            new SortDescription(nameof(PatientListItemViewModel.FullName), ListSortDirection.Ascending));

        PatientCollectionView.Filter = o =>
        {
            if (o is PatientListItemViewModel p)
            {
                return p.Surname.ToLower().StartsWith(PatientSearchFilter.ToLower()) ||
                       p.Name.ToLower().StartsWith(PatientSearchFilter.ToLower());
            }

            return false;
        };

        AddPatientCommand = new RelayCommand<object>(_ =>
            navigationService.NavigateTo(ViewType.PatientCreate, null));

        EditPatientCommand = new RelayCommand<int>(id =>
            navigationService.NavigateTo(ViewType.PatientUpdate, id));

        ShowPatientCommand = new RelayCommand<int>(id =>
            navigationService.NavigateTo(ViewType.PatientInfo, id));

        ExportVisitsCommand = new AsyncRelayCommand(ExportVisits);

        LoadedCommand = new AsyncRelayCommand(LoadData);
    }

    private async Task ExportVisits()
    {
        await _exportService.ExportVisitsAsync(
            new DateOnly(2023, 9, 1),
            new DateOnly(2023, 10, 17),
            _exportOptions.DefaultDirPath);
    }

    private async Task LoadData()
    {
        foreach (var patient in await GetPatientsAsync())
        {
            _patients.Add(patient);
        }
    }

    private async Task<List<PatientListItemViewModel>> GetPatientsAsync()
    {
        return (await _patientService.GetAllAsync())
            .Select(p => new PatientListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Surname = p.Surname,
                Patronymic = p.Patronymic
            })
            .ToList();
    }
}