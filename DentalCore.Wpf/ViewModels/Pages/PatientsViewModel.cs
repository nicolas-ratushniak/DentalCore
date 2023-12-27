using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Abstract;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Pages;

public class PatientsViewModel : BaseViewModel
{
    private readonly IPatientService _patientService;
    private string _patientSearchFilter = string.Empty;
    private readonly ObservableCollection<PatientListItemViewModel> _patients;

    public ICommand GoToPatientCreateCommand { get; }
    public ICommand GoToPatientUpdateCommand { get; }
    public ICommand GoToPatientInfoCommand { get; }
    public ICommand GoToVisitsExportCommand { get; }

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

    public PatientsViewModel(INavigationService navigationService, IPatientService patientService)
    {
        _patientService = patientService;
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

        GoToPatientCreateCommand = new RelayCommand<object>(_ =>
            navigationService.NavigateTo(PageType.PatientCreate, null));

        GoToPatientUpdateCommand = new RelayCommand<int>(id =>
            navigationService.NavigateTo(PageType.PatientUpdate, id));

        GoToPatientInfoCommand = new RelayCommand<int>(id =>
            navigationService.NavigateTo(PageType.PatientInfo, id));

        GoToVisitsExportCommand = new RelayCommand<object>(_ =>
            navigationService.NavigateTo(PageType.VisitsExport, null));

        LoadedCommand = new AsyncRelayCommand(LoadData);
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